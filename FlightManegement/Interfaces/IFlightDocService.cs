using FlightManegement.Models;

namespace FlightManegement.Interfaces
{
    public interface IFlightDocService
    {
        Task<FlightDoc> CreateFlightDocAsync(int flightId, string documentName, string type, string creator);
        Task<FlightDoc> UpdateFlightDocAsync(int flightDocId, string documentName, double version, string filePath);
        Task<bool> DeleteFlightDocAsync(int flightDocId);
        Task<byte[]> GetFlightDocPdfAsync(int flightDocId);
    }
}
