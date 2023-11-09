using FlightManegement.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FlightManegement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightAssignmentController : ControllerBase
    {
        private readonly IFlightAssignmentService _flightAssignmentService;

        public FlightAssignmentController(IFlightAssignmentService flightAssignmentService)
        {
            _flightAssignmentService = flightAssignmentService;
        }

        [HttpPost("AddMember")]
        public async Task<IActionResult> AddMemberForFlight(int userGroupId, int flightId)
        {
            var result = await _flightAssignmentService.AddMemberForFlight(userGroupId, flightId);
            if (result)
            {
                return Ok("Thành viên đã được thêm vào chuyến bay thành công.");
            }
            return BadRequest("Không thể thêm thành viên vào chuyến bay.");
        }

        [HttpDelete("RemoveMember")]
        public async Task<IActionResult> RemoveUserFromFlight(int userGroupId, int flightId)
        {
            try
            {
                var result = await _flightAssignmentService.RemoveUserFromFlight(userGroupId, flightId);
                return Ok("Thành viên đã được xóa khỏi chuyến bay.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi xóa thành viên khỏi chuyến bay: {ex.Message}");
            }
        }

        [HttpGet("GetPilots/{flightId}")]
        public async Task<IActionResult> GetPilotsForFlight(int flightId)
        {
            var pilots = await _flightAssignmentService.GetPilotsForFlight(flightId);
            if (pilots != null)
            {
                return Ok(pilots);
            }
            return NotFound("Không tìm thấy phi công nào cho chuyến bay này.");
        }

        [HttpGet("GetCrew/{flightId}")]
        public async Task<IActionResult> GetCrewForFlight(int flightId)
        {
            var crewMembers = await _flightAssignmentService.GetCrewForFlight(flightId);
            if (crewMembers != null)
            {
                return Ok(crewMembers);
            }
            return NotFound("Không tìm thấy thành viên tổ bay nào cho chuyến bay này.");
        }

        [HttpGet("GetAllMembers/{flightId}")]
        public async Task<IActionResult> GetAllMembersForFlight(int flightId)
        {
            var members = await _flightAssignmentService.GetAllMembersForFlight(flightId);
            if (members != null)
            {
                return Ok(members);
            }
            return NotFound("Không tìm thấy thành viên nào cho chuyến bay này.");
        }
    }
}
