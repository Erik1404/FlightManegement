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
            var pdfData = await _pdfService.GenerateFlightInfoPdf(flightId);
            // Lưu trữ PDF vào hệ thống tệp và lấy đường dẫn
            var filePath = SavePdfToFileSystem(pdfData, documentName);

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
            var flightDoc = await _dbContext.FlightDocs.FindAsync(flightDocId);
            if (flightDoc == null)
                throw new InvalidOperationException("FlightDoc not found.");

            flightDoc.DocumentName = documentName;
            flightDoc.Version = version;
            flightDoc.FilePath = filePath;

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
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var storageFolder = Path.Combine(documentsPath, "MyFlightDocs");

            Directory.CreateDirectory(storageFolder); 
            var uniqueFileName = $"{Guid.NewGuid()}_{documentName}.pdf";
            var filePath = Path.Combine(storageFolder, uniqueFileName);
            File.WriteAllBytes(filePath, pdfData);
            return filePath;
        }
    }

/*    Tham khảo pdf itextsharp https://www.youtube.com/watch?v=sQOf8qVYaX0&list=PL9VntEIxiRgd3yNTIIsYO1znwa5Z1qrEf
*/}
