using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightManegement.Data;
using FlightManegement.Interfaces;
using FlightManegement.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightManagement.Services
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
            return await _dbContext.Set<Flight>().ToListAsync();
        }
        public async Task<Flight> GetFlightById(int id)
        {
            return await _dbContext.Flights.FirstOrDefaultAsync(f => f.FlightId == id);
        }

        public List<Flight> SearchFlight(string keyword)
        {
            return _dbContext.Set<Flight>()
                           .Where(f => f.FlightName.Contains(keyword) ||
                                       f.FlightNumber.Contains(keyword))
                           .ToList();
        }

        public async Task<Flight> AddFlight(string depart, string destination, DateTime departureTime, DateTime arrivalTime, string notes)
        {
            var flightname = "VietJetAir";
            // Tìm số lớn nhất của FlightNumber
            var flightNumbers = _dbContext.Flights
                .Select(fn => fn.FlightNumber)
                .ToList();

            int maxFlightNumber = flightNumbers
                .Select(fn => int.TryParse(fn.Substring(2), out int number) ? number : 0)
                .DefaultIfEmpty(0)
                .Max();

            // Tạo FlightNumber với định dạng "VJ + {001}" tăng dần
            string newFlightNumber = $"VJ{(maxFlightNumber + 1):D3}";
            var newFlight = new Flight
            {
               FlightName = flightname,
               FlightNumber = newFlightNumber,
               Depart = depart,
               Destination = destination,
               DepartureTime = departureTime,
               ArrivalTime = arrivalTime,
               Notes = notes
            };

            _dbContext.Flights.Add(newFlight);
            await _dbContext.SaveChangesAsync();

            return newFlight;
        }

        public async Task<bool> UpdateFlight(int flightId, string flightName, string flightNumber, string depart, string destination, DateTime departureTime, DateTime arrivalTime, FlightStatus status, string notes)
        {
            var flight = await _dbContext.Flights.FindAsync(flightId);
            if (flight == null)
            {
                return false;
            }

            // Cập nhật thông tin chuyến bay
            flight.FlightName = flightName;
            flight.FlightNumber = flightNumber;
            flight.Depart = depart;
            flight.Destination = destination;
            flight.DepartureTime = departureTime;
            flight.ArrivalTime = arrivalTime;
            flight.Status = status;
            flight.Notes = notes;

            _dbContext.Entry(flight).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _dbContext.Flights.AnyAsync(f => f.FlightId == flightId))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }


        public async Task<bool> DeleteFlight(int flightId)
        {
            var flight = await _dbContext.Set<Flight>().FindAsync(flightId);
            if (flight != null)
            {
                _dbContext.Set<Flight>().Remove(flight);
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
