using FlightManegement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlightManegement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightDocsController : ControllerBase
    {
        private readonly IFlightDocService _flightDocService;

        public FlightDocsController(IFlightDocService flightDocService)
        {
            _flightDocService = flightDocService;
        }

        [HttpPost("create")]
        [Authorize] 
        public async Task<IActionResult> CreateFlightDoc(int flightId, string documentName, string type)
        {
            if (User.HasClaim(ClaimTypes.Role, "Admin") || User.HasClaim(ClaimTypes.Role, "Pilot"))
            {

                var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(userName))
                {
                    return Unauthorized("Không thể xác định thông tin người dùng.");
                }

                if (string.IsNullOrWhiteSpace(documentName) || string.IsNullOrWhiteSpace(type))
                {
                    return BadRequest("Cần cung cấp đầy đủ thông tin tên tài liệu và loại tài liệu.");
                }

                try
                {
                    var flightDoc = await _flightDocService.CreateFlightDocAsync(flightId, documentName, type, userName);

                    return CreatedAtAction(nameof(GetFlightDoc), new { flightDocId = flightDoc.FlightDocId }, flightDoc);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Có lỗi xảy ra khi tạo tài liệu mới: " + ex.Message);
                }
            }
            return Unauthorized(new { message = "Bạn không có quyền thực hiện tác vụ này" });
        }


        [HttpGet("{flightDocId}")]
        public async Task<IActionResult> GetFlightDoc(int flightDocId)
        {
            try
            {
                var pdfData = await _flightDocService.GetFlightDocPdfAsync(flightDocId);
                if (pdfData == null)
                {
                    return NotFound();
                }

                return File(pdfData, "application/pdf");
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{flightDocId}")]
        public async Task<IActionResult> DeleteFlightDoc(int flightDocId)
        {
            var success = await _flightDocService.DeleteFlightDocAsync(flightDocId);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut("{flightDocId}")]
        public async Task<IActionResult> UpdateFlightDoc(int flightDocId, string documentName, double version, string filePath)
        {
            if (string.IsNullOrWhiteSpace(documentName) || string.IsNullOrWhiteSpace(filePath))
            {
                return BadRequest("Document name and file path are required.");
            }

            try
            {
                var flightDoc = await _flightDocService.UpdateFlightDocAsync(flightDocId, documentName, version, filePath);
                return Ok(flightDoc);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, ex.Message);
            }
        }

    }

}
