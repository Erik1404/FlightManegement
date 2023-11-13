using FlightManegement.Data;
using FlightManegement.Interfaces;
using FlightManegement.Models;

namespace FlightManegement.Services
{
    public class FlightDocService : IFlightDocService
    {
        private readonly FlightManagementDbContext _dbContext;
        private readonly IPdfService _pdfService;

        public FlightDocService(FlightManagementDbContext dbContext, IPdfService pdfService)
        {
            _dbContext = dbContext;
            _pdfService = pdfService;
        }

        public async Task<FlightDoc> CreateFlightDocAsync(int flightId, string documentName, string type, string creator)
        {
            // Tạo PDF với thông tin chuyến bay
            var pdfData = await _pdfService.GenerateFlightInfoPdf(flightId);

            // Lưu trữ PDF vào hệ thống tệp và lấy đường dẫn
            var filePath = SavePdfToFileSystem(pdfData, documentName);

            // Tạo đối tượng FlightDoc mới và lưu vào cơ sở dữ liệu
            var flightDoc = new FlightDoc
            {
                FlightId = flightId,
                DocumentName = documentName,
                Type = type,
                CreateDate = DateTime.Now,
                Creator = creator,
                Version = 1.0,
                FilePath = filePath
            };
            _dbContext.FlightDocs.Add(flightDoc);
            await _dbContext.SaveChangesAsync();

            return flightDoc;
        }

        public async Task<FlightDoc> UpdateFlightDocAsync(int flightDocId, string documentName, double version, string filePath)
        {
            // Tìm và cập nhật thông tin của FlightDoc
            var flightDoc = await _dbContext.FlightDocs.FindAsync(flightDocId);
            if (flightDoc == null)
                throw new InvalidOperationException("FlightDoc not found.");

            flightDoc.DocumentName = documentName;
            flightDoc.Version = version;
            flightDoc.FilePath = filePath; // Chỉ cập nhật nếu có file mới được tạo

            _dbContext.FlightDocs.Update(flightDoc);
            await _dbContext.SaveChangesAsync();

            return flightDoc;
        }

        public async Task<bool> DeleteFlightDocAsync(int flightDocId)
        {
            var flightDoc = await _dbContext.FlightDocs.FindAsync(flightDocId);
            if (flightDoc == null)
                return false;

            _dbContext.FlightDocs.Remove(flightDoc);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<byte[]> GetFlightDocPdfAsync(int flightDocId)
        {
            var flightDoc = await _dbContext.FlightDocs.FindAsync(flightDocId);
            if (flightDoc == null)
                throw new InvalidOperationException("FlightDoc not found.");

            var pdfData = System.IO.File.ReadAllBytes(flightDoc.FilePath);
            return pdfData;
        }

        private string SavePdfToFileSystem(byte[] pdfData, string documentName)
        {
            // Lấy đường dẫn đến thư mục MyDocuments của người dùng hiện tại.
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Đặt tên thư mục con mà bạn muốn lưu trữ tài liệu PDF.
            var storageFolder = Path.Combine(documentsPath, "MyFlightDocs");

            // Đảm bảo rằng thư mục tồn tại.
            Directory.CreateDirectory(storageFolder); // Nếu thư mục đã tồn tại, hàm này sẽ không tạo mới.
            var uniqueFileName = $"{Guid.NewGuid()}_{documentName}.pdf";
            var filePath = Path.Combine(storageFolder, uniqueFileName);
            File.WriteAllBytes(filePath, pdfData);
            return filePath;
        }


        // Các phương thức khác nếu cần
    }

}
