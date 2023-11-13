using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Font;
using iText.Kernel.Font;
using Microsoft.EntityFrameworkCore;
using FlightManegement.Data;
using FlightManegement.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class PdfService : IPdfService
{
    private readonly FlightManagementDbContext _dbContext;

    public PdfService(FlightManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<byte[]> GenerateFlightInfoPdf(int flightId)
    {
        var flightAssignments = await _dbContext.FlightAssignments
            .Include(fa => fa.Flight)
            .Include(fa => fa.UserGroup)
                .ThenInclude(ug => ug.User)
            .Where(fa => fa.FlightId == flightId)
            .ToListAsync();

        if (!flightAssignments.Any())
        {
            throw new InvalidOperationException("Không tìm thấy chuyến bay hoặc không có thông tin phân công.");
        }

        var flight = flightAssignments.First().Flight;

        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new PdfWriter(memoryStream))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    var document = new Document(pdf);
                    // Đường dẫn đến font hỗ trợ tiếng Việt cần được chỉ định chính xác
                    string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
                    PdfFont font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
                    document.SetFont(font);

                    document.Add(new Paragraph("Thông tin chuyến bay").SetFont(font).SetBold().SetFontSize(14));
                    document.Add(new Paragraph($"Hãng bay: {flight.FlightName}").SetFont(font));
                    document.Add(new Paragraph($"Thông tin chuyến bay: {flight.FlightNumber}").SetFont(font));
                    document.Add(new Paragraph($"Đi từ: {flight.Depart}").SetFont(font));
                    document.Add(new Paragraph($"Điểm đến: {flight.Destination}").SetFont(font));
                    document.Add(new Paragraph($"Thời gian: Từ {flight.DepartureTime:dd/MM/yyyy HH:mm} đến {flight.ArrivalTime:dd/MM/yyyy HH:mm}").SetFont(font));

                    // Truy vấn thông tin phi công, bao gồm cả tên đăng nhập và số điện thoại.
                    var pilotDetails = flightAssignments
                        .Where(fa => fa.UserGroup != null && fa.UserGroup.User != null && fa.UserGroup.User.Role == "Pilot")
                        .Select(fa => new
                        {
                            Username = fa.UserGroup.User.Username,
                            PhoneNumber = fa.UserGroup.User.PhoneNumber
                        })
                        .ToList();

                    // Kiểm tra xem có thông tin phi công nào không và thêm vào tài liệu.
                    if (pilotDetails.Any())
                    {
                        document.Add(new Paragraph("Phi công:").SetFont(font).SetBold());
                        foreach (var pilot in pilotDetails)
                        {
                            // Kết quả hiển thị sẽ là: Tên User - Số Điện Thoại: PhoneNumber
                            document.Add(new Paragraph($"{pilot.Username} - Số Điện Thoại: {pilot.PhoneNumber}").SetFont(font));
                        }
                    }
                    else
                    {
                        document.Add(new Paragraph("Không có thông tin phi công.").SetFont(font).SetItalic());
                    }

                    var crewMembers = flightAssignments
                        .Where(fa => fa.UserGroup != null && fa.UserGroup.User != null && fa.UserGroup.User.Role == "Crew")
                        .Select(fa => fa.UserGroup.User.Username)
                        .ToList();

                    if (crewMembers.Any())
                    {
                        document.Add(new Paragraph("Tiếp viên:").SetFont(font).SetBold());
                        crewMembers.ForEach(member => document.Add(new Paragraph(member).SetFont(font)));
                    }
                    else
                    {
                        document.Add(new Paragraph("Không có thông tin tiếp viên.").SetFont(font).SetItalic());
                    }

                    document.Close();
                }
            }
            return memoryStream.ToArray();
        }
    }
}
