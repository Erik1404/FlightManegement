using FlightManegement.Models;

namespace FlightManegement.Interfaces
{
    public interface IFlightService
    {
        Task<List<Flight>> GetAllFlights();
        List<Flight> SearchFlight(string keyword);
        Task<Flight> AddFlight(Flight Flight);
        Task<bool> DeleteFlight(int FlightId);
        Task<bool> UpdateFlight(Flight Flight);
    }
}
