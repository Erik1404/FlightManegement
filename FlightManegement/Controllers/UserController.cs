using Microsoft.AspNetCore.Mvc;
using FlightManegement.Services;
using System;
using System.Threading.Tasks;
using FlightManegement.Interfaces;
using FlightManegement.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FlightManegement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(string username, string password, string email, string address, string phoneNumber, DateTime dateOfBirth, string confirmpassword)
        {
            try
            {
                var result = await _userService.Register(username, password, email, address, phoneNumber, dateOfBirth, confirmpassword);
                if (result == null)
                {
                    return BadRequest("User registration failed.");
                }

                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"User registration failed: {ex.Message}");
            }
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var token = await _userService.Login(username, password);
                if (token == null)
                {
                    return Unauthorized("Sai tên đăng nhập hoặc mật khẩu");
                }

                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest($"Login failed: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateUser(int id, UserDto userDto)
        {
            if (User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                var updatedUser = _userService.UpdateUser(id, userDto);
                if (updatedUser == null)
                {
                    return NotFound();
                }
                return Ok(updatedUser);
            }
            return Unauthorized(new { message = "Bạn không có quyền thực hiện tác vụ này" });
        }


        /*   [HttpPost("Refresh-Token")]
           public async Task<IActionResult> RefreshToken()
           {
               try
               {
                   User currentUser = HttpContext.Items["user"] as User;
                   var token = await _userService.RefreshUserTokenAsync(currentUser);
                   return Ok(token);
               }
               catch (Exception ex)
               {
                   return BadRequest($"Token refresh failed: {ex.Message}");
               }
           }*/
    }
}
