using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FlightManagement.Services;
using FlightManegement.Interfaces;
using FlightManegement.Models;
using FlightManegement.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FlightManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightsController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        // GET: api/Flights
        [HttpGet]
        public async Task<IActionResult> GetAllFlights()
        {
            var flights = await _flightService.GetAllFlights();
            return Ok(flights);
        }

        // GET: api/Flights/search?keyword=abc
        [HttpGet("search")]
        public IActionResult SearchFlights(string keyword)
        {
            var flights = _flightService.SearchFlight(keyword);
            return Ok(flights);
        }

    
        [HttpPost("Add Flight")]
        [Authorize]
        public async Task<ActionResult<Flight>> AddFlight(string depart, string destination, DateTime departureTime, DateTime arrivalTime, string notes)
        {
            // Với nhiều Role thì if (User.HasClaim(ClaimTypes.Role, "Admin") || User.HasClaim(ClaimTypes.Role, "Pilot"))
            if (User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                var add = await _flightService.AddFlight(depart, destination, departureTime, arrivalTime, notes);
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



        // GET: api/Flights/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlight(int id)
        {
            var flight = await _flightService.GetFlightById(id);
            if (flight == null)
            {
                return NotFound();
            }
            return Ok(flight);
        }

        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            var result = await _flightService.DeleteFlight(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("Update Flight/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateFlight(int id, string flightName, string flightNumber, string depart, string destination, DateTime departureTime, DateTime arrivalTime, FlightStatus status, string notes)
        {
            if (User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                var result = await _flightService.UpdateFlight(id, flightName, flightNumber, depart, destination, departureTime, arrivalTime, status, notes);

                if (result)
                {
                    return Ok(new { message = "Cập nhật chuyến bay thành công" });
                }
                else
                {
                    return NotFound(new { message = "Chuyến bay không được tìm thấy" });
                }
            }

            // Xử lý trường hợp người dùng không có role "Admin"
            return Unauthorized(new { message = "Bạn không có quyền thực hiện tác vụ này" });
        }
    }
}
