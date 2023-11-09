using FlightManegement.Models;

namespace FlightManegement.Interfaces
{
    public interface IFlightService
    {
        Task<List<Flight>> GetAllFlights();
        Task<Flight> GetFlightById(int id);
        List<Flight> SearchFlight(string keyword);
        Task<Flight> AddFlight(string depart, string destination, DateTime departureTime, DateTime arrivalTime, string notes);
        Task<bool> UpdateFlight(int flightId, string flightName, string flightNumber, string depart, string destination, DateTime departureTime, DateTime arrivalTime, FlightStatus status, string notes);
        Task<bool> DeleteFlight(int FlightId);

    }
}
