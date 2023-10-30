using FlightManegement.Interfaces;
using FlightManegement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightManegement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public static User user = new User();
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;

        }



        [HttpPost("register")]
        public IActionResult Register(string userName, string email, string password, string confirmPassword, DateTime dateOfBirth, string address)
        {
            var registeredUser = _userService.Register(userName, email, password, confirmPassword, dateOfBirth, address);

            if (registeredUser == null)
            {
                return BadRequest(new { message = "Email đã tồn tại." });
            }

            return Ok(registeredUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string userNameOrEmail, string password)
        {
            try
            {
                User userlogin = await _userService.LoginAsync(userNameOrEmail, password);
                return Ok(userlogin);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
