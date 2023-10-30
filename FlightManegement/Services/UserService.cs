using FlightManegement.Data;
using FlightManegement.Models;
using FlightManegement.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace FlightManegement.Services
{
    public class UserService : IUserService
    {
        private readonly FlightManagementDbContext _dbContext;

        public UserService(FlightManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public User Register(string userName, string email, string password,string confirmpassword, DateTime dateOfBirth, string address)
        {
            // Kiểm tra định dạng email
            if (!IsValidEmail(email))
            {
                throw new Exception("Email không hợp lệ.");
            }
            // Kiểm tra xác nhận mật khẩu
            if (password != confirmpassword)
            {
                throw new Exception("Mật khẩu và xác nhận mật khẩu không khớp.");
            }

            if (_dbContext.Users.Any(u => u.UserName == userName || u.Email == email))
            {
                throw new Exception("Tài khoản này có vẻ đã được đăng ký");
            }
            string role;
            string usercode;
            if (email.EndsWith("@Admin.com"))
            {
                role = "Admin";
                // Tìm số lớn nhất của UserID cho Role là "Admin"
                int maxUserId = _dbContext.Users
                    .Where(u => u.Role == "Admin")
                    .Max(u => (int?)u.UserId) ?? 0;

                // Tạo UserCode cho "Admin" với định dạng "Admin + {001}" tăng dần
                usercode = $"Admin{maxUserId + 1:D3}";
            }
            else
            {
                role = "Nhân viên này chưa được bổ nhiệm chức vụ nào";
                usercode = "Chưa có chức vụ";
            }
            var user = new User
            {
                UserCode = usercode,
                UserName = userName,
                Email = email,
                Password = HashPassword(password), // Mã hóa mật khẩu
                DateOfBirth = dateOfBirth,
                Address = address,
                Role = role,
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            return user;
        }


        public async Task<User> LoginAsync(string userNameOrEmail, string password)
        {
            // Kiểm tra xem `userNameOrEmail` có phải là UserName hoặc Email
            var user = await _dbContext.Users
                .Where(u => u.UserName == userNameOrEmail || u.Email == userNameOrEmail)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại.");
            }

            // Kiểm tra mật khẩu
            if (user.Password != HashPassword(password))
            {
                throw new Exception("Mật khẩu không đúng.");
            }

            // Trả về thông tin người dùng sau khi đăng nhập
            return user;
        }

        // Phương thức để kiểm tra định dạng email
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Phương thức để mã hóa mật khẩu
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
