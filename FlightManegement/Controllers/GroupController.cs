﻿using FlightManegement.Interfaces;
using FlightManegement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace FlightManegement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _GroupService;
        public GroupController(IGroupService GroupService)
        {
            _GroupService = GroupService;
        }

        [HttpGet("GetAllGroup")]
        public async Task<ActionResult<List<Group>>> GetAllGroups()
        {
            try
            {
                var Groups = await _GroupService.GetAllGroups();
                return Ok(Groups);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Search Group")]
        public IActionResult SearchGroups(string keyword)
        {
            var Groups = _GroupService.SearchGroup(keyword);
            return Ok(Groups);
        }

        [HttpPost("Add Group")]
        [Authorize]
        public async Task<ActionResult<Group>> AddGroup(string Group_Name, string Group_Desc)
        {
            // Với nhiều Role thì if (User.HasClaim(ClaimTypes.Role, "Admin") || User.HasClaim(ClaimTypes.Role, "Pilot"))
            if (User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                var add = await _GroupService.AddGroup(Group_Name, Group_Desc);
                if (add != null)
                {
                    return Ok(new { message = "Thêm nhóm mới thành công", Group = add });
                }
                else
                {
                    return BadRequest(new { message = "Thêm nhóm mới thất bại" });
                }
            }

            // Xử lý trường hợp người dùng không có role "Admin"
            return Unauthorized(new { message = "Bạn không có quyền thực hiện tác vụ này" });
        }


        [HttpDelete("Delete Group")]
        public async Task<ActionResult> DeleteGroup(int GroupId)
        {
            var deleted = await _GroupService.DeleteGroup(GroupId);
            if (deleted)
            {
                return Ok(new { message = "Xóa nhóm thành công" });
            }
            else
            {
                return NotFound(new { message = "Không tìm thấy data" });
            }
        }


        [HttpPut("Update Group")]
        public async Task<ActionResult> UpdateGroup(int GroupId, Group Group)
        {
            if (GroupId != Group.GroupId)
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });

            var updated = await _GroupService.UpdateGroup(Group);
            if (updated)
            {
                return Ok(new { message = "Cập nhật nhóm thành công" });
            }
            else
            {
                return NotFound(new { message = "Không tìm thấy dữ liệu" });
            }
        }

    }
}
