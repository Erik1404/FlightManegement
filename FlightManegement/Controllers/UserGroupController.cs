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

        [HttpPost("addusertogroup")]
        [Authorize]
        public async Task<IActionResult> AddUserToGroup(int userId,int groupId)
        {
            if (User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                bool result = await _userGroupService.AddUserToGroupAsync(userId, groupId);

                if (result)
                {
                    return Ok("Thêm Thành công");
                }

                return BadRequest("Lỗi khi thêm nhân viên này vào nhóm");
            }
            return Unauthorized(new { message = "Bạn không có quyền thực hiện tác vụ này" });
        }
    }
}
