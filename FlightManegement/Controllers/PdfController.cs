using Microsoft.AspNetCore.Mvc;
using FlightManegement.Interfaces;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class FlightPdfController : ControllerBase
{
    private readonly IPdfService _pdfService;

    public FlightPdfController(IPdfService pdfService)
    {
        _pdfService = pdfService;
    }

    // GET: /FlightPdf/ID
    [HttpGet("{flightId}")]
    public async Task<IActionResult> GetFlightInfoAsPdf(int flightId)
    {
        try
        {
            var pdfBytes = await _pdfService.GenerateFlightInfoPdf(flightId);
            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                return NotFound("Không thể tạo tài liệu PDF cho chuyến bay này.");
            }
            return File(pdfBytes, "application/pdf", $"FlightInfo_{flightId}.pdf");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
