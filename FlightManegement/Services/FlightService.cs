using FlightManegement.Data;
using FlightManegement.Interfaces;
using FlightManegement.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightManegement.Services
{
    public class FlightService : IFlightService
    {
        private readonly FlightManagementDbContext _dbContext;

        public FlightService(FlightManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<List<Flight>> GetAllFlights()
        {
            return await _dbContext.Flights.ToListAsync();
        }

        public List<Flight> SearchFlight(string keyword)
        {
            return _dbContext.Flights
                .Where(f => f.FlightName.Contains(keyword) ||
                            f.FlightBrand.Contains(keyword) ||
                            f.DepartureAirport.Contains(keyword) ||
                            f.ArrivalAirport.Contains(keyword))
                .ToList();
        }

        public async Task<Flight> AddFlight(Flight flight)
        {
            _dbContext.Flights.Add(flight);
            await _dbContext.SaveChangesAsync();
            return flight;
        }

        public async Task<bool> DeleteFlight(int flightId)
        {
            var flight = await _dbContext.Flights.FindAsync(flightId);
            if (flight != null)
            {
                _dbContext.Flights.Remove(flight);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateFlight(Flight flight)
        {
            var existingFlight = await _dbContext.Flights.FindAsync(flight.FlightId);
            if (existingFlight != null)
            {
                existingFlight.FlightName = flight.FlightName;
                existingFlight.FlightBrand = flight.FlightBrand;
                existingFlight.DepartureAirport = flight.DepartureAirport;
                existingFlight.ArrivalAirport = flight.ArrivalAirport;
                existingFlight.DepartureTime = flight.DepartureTime;
                existingFlight.ArrivalTime = flight.ArrivalTime;

                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
