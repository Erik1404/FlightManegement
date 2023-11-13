namespace FlightManegement.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerateFlightInfoPdf(int flightId);
    }
}
