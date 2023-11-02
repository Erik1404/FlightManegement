using FlightManegement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlightManegement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupController : ControllerBase
    {
        private readonly IUserGroupService _userGroupService;

        public UserGroupController(IUserGroupService userGroupService)
        {
            _userGroupService = userGroupService;
        }

        [HttpPost("addusertogroup"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUserToGroup(int userId,int groupId)
        {

                bool result = await _userGroupService.AddUserToGroupAsync(userId, groupId);

                if (result)
                {
                    return Ok("User added to group successfully.");
                }

                return BadRequest("Failed to add user to group.");

        }
    }
}
