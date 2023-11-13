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
        public DbSet<User> Users { get; set; }

        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<FlightAssignment> FlightAssignments { get; set; }
        public DbSet<FlightDoc> FlightDocs { get; set; }


    }
}
