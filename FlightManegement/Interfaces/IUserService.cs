using FlightManegement.Models;
using System.Net.NetworkInformation;

namespace FlightManegement.Interfaces
{
    public interface IUserService
    {
        User Register(string userName, string email, string password, string confirmPassword, DateTime dateOfBirth, string address);
        Task<User> LoginAsync(string userNameOrEmail, string password);
     
    }
}
