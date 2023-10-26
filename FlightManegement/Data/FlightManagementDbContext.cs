using FlightManegement.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightManegement.Data
{
    public class FlightManagementDbContext : DbContext
    {
        public FlightManagementDbContext(DbContextOptions<FlightManagementDbContext> options) : base(options)
        {

        }

        public DbSet<Flight> Flights { get; set; }

    }
}
