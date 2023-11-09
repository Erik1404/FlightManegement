using FlightManegement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace FlightManegement.Interfaces
{
    public interface IUserService
    {
        Task<User> Register(string username, string password, string email, string address, string phoneNumber, DateTime dateOfBirth, string confirmPassword);
        Task<IActionResult> Login(string username, string password);
        Task<IActionResult> RefreshUserTokenAsync(User user);
        Task<bool> UpdateUser(int userId, UserDto userDto);

    }
}
