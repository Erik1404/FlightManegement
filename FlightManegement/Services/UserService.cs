using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlightManegement.Data;
using FlightManegement.Interfaces;
using FlightManegement.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightManegement.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly FlightManagementDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public UserService(IHttpContextAccessor httpContextAccessor, FlightManagementDbContext dbContext, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<User> Register(string username, string password, string email, string address, string phoneNumber, DateTime dateOfBirth, string confirmPassword)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phoneNumber))
            {
                throw new Exception("Thiếu thông tin");
            }

            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username || u.Email == email);
            if (existingUser != null)
            {
                throw new Exception("Tài khoản này đã được đăng ký.");
            }

            // Tạo hash mật khẩu
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            string role;
            string userCode;
            if (email.EndsWith("@Admin.com"))
            {
                role = "Admin";

                var adminUsers = _dbContext.Users.Where(u => u.Role == "Admin").AsEnumerable();

                // Tìm số lớn nhất của UserCode cho Role là "Admin"
                int maxAdminCodeNumber = adminUsers
                    .Select(u => int.Parse(u.UserCode.Substring("Admin".Length)))
                    .DefaultIfEmpty(0)
                    .Max();

                // Tạo UserCode cho "Admin" với định dạng "Admin + {001}" tăng dần
                userCode = $"Admin{maxAdminCodeNumber + 1:D3}"; // D3 để đảm bảo có ba chữ số
            }
            else
            {
                role = "Nhân viên này chưa được bổ nhiệm chức vụ nào";
                userCode = "Chưa có";
            }

            // Tạo đối tượng User mới
            var newUser = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Email = email,
                Address = address,
                PhoneNumber = phoneNumber,
                DateOfBirth = dateOfBirth,
                UserCode = userCode,
                Role = role,
            };

            // Kiểm tra xác nhận mật khẩu
            if (password != confirmPassword)
            {
                throw new Exception("Mật khẩu và xác nhận mật khẩu không khớp.");
            }

            // Lưu người dùng vào cơ sở dữ liệu
            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
            return newUser;
        }

        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("Mật khẩu không đúng.");
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new Exception("Tên đăng nhập hoặc Email không tồn tại.");
            }

            // Tìm kiếm người dùng trong cơ sở dữ liệu
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (existingUser == null)
            {
                throw new ArgumentException("Invalid username or password.");
            }

            // Kiểm tra mật khẩu
            if (!VerifyPasswordHash(password, existingUser.PasswordHash, existingUser.PasswordSalt))
            {
                throw new ArgumentException("Invalid username or password.");
            }

            string token = CreateToken(existingUser);

            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken, existingUser);

            return new ObjectResult(token) { StatusCode = 200 };
        }


        public async Task<IActionResult> RefreshUserTokenAsync(User user)
        {
            if (user == null)
            {
                return new ObjectResult("User is null") { StatusCode = 400 };
            }
            var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];

            if (!user.RefreshToken.Equals(refreshToken))
            {
                return new ObjectResult("Invalid Refresh Token") { StatusCode = 401 };
            }
            else if (user.TokenExpires < DateTime.Now)
            {
                return new ObjectResult("Token expired") { StatusCode = 401 };
            }

            string token = CreateToken(user);
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken, user);

            return new ObjectResult(token) { StatusCode = 200 };
        }



        // Update user 
        public async Task<bool> UpdateUser(int userId, UserDto userDto)
        {
            // Tìm kiếm người dùng cần cập nhật
            var user = await _dbContext.Users.FindAsync(userId);

            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            // Cập nhật thông tin người dùng từ UserDto
            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.Address = userDto.Address;
            user.PhoneNumber = userDto.PhoneNumber;
            user.DateOfBirth = userDto.DateOfBirth;

            // Lưu thay đổi vào cơ sở dữ liệu
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }


        // Khu vực xử lý token 
        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }


        private void SetRefreshToken(RefreshToken newRefreshToken, User user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
