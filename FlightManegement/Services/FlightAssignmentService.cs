﻿using FlightManegement.Data;
using FlightManegement.Interfaces;
using FlightManegement.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FlightManegement.Services
{
    public class FlightAssignmentService : IFlightAssignmentService
    {
        private readonly FlightManagementDbContext _dbContext;

        public FlightAssignmentService(FlightManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddMemberForFlight(int userGroupId, int flightId)
        {
            var userGroup = await _dbContext.UserGroups
                                            .Include(ug => ug.User)
                                            .FirstOrDefaultAsync(ug => ug.UserGroupId == userGroupId);
            if (userGroup == null) return false;

            var role = userGroup.User.Role;
            var count = await _dbContext.FlightAssignments
                                        .CountAsync(fa => fa.FlightId == flightId && fa.UserGroup.User.Role == role);

            // Logic for limiting the number of pilots and crew members...
            // ...

            // Retrieve the flight to check its departure and arrival times
            var flightToAssign = await _dbContext.Flights.FindAsync(flightId);
            if (flightToAssign == null) return false;

            // Check for time conflicts with existing assignments for the user
            var assignmentsForUser = await _dbContext.FlightAssignments
                                                     .Include(fa => fa.Flight)
                                                     .Where(fa => fa.UserGroupId == userGroupId)
                                                     .ToListAsync();

            foreach (var assignment in assignmentsForUser)
            {
                if (flightToAssign.DepartureTime < assignment.Flight.ArrivalTime &&
                    flightToAssign.ArrivalTime > assignment.Flight.DepartureTime)
                {
                    return false; // There is a time conflict with another flight
                }
            }

            // No time conflicts, proceed with assignment
            var newAssignment = new FlightAssignment
            {
                UserGroupId = userGroupId,
                FlightId = flightId
            };

            _dbContext.FlightAssignments.Add(newAssignment);
            await _dbContext.SaveChangesAsync();

            return true;
        }


        public async Task<bool> RemoveUserFromFlight(int userGroupId, int flightId)
        {
            var assignment = await _dbContext.FlightAssignments
                                             .FirstOrDefaultAsync(fa => fa.UserGroupId == userGroupId && fa.FlightId == flightId);

            if (assignment == null)
            {
                throw new Exception("Không tìm thấy tài liệu chuyến bay");
            }

            _dbContext.FlightAssignments.Remove(assignment);
            await _dbContext.SaveChangesAsync();

            return true;
        }


        public async Task<List<User>> GetPilotsForFlight(int flightId)
        {
            var pilotAssignments = await _dbContext.FlightAssignments
                .Include(fa => fa.UserGroup)
                .ThenInclude(ug => ug.User)
                .Where(fa => fa.FlightId == flightId && fa.UserGroup.User.Role == "Pilot")
                .ToListAsync();

            return pilotAssignments.Select(fa => fa.UserGroup.User).ToList();
        }

        public async Task<List<User>> GetCrewForFlight(int flightId)
        {
            var crewAssignments = await _dbContext.FlightAssignments
                .Include(fa => fa.UserGroup)
                .ThenInclude(ug => ug.User)
                .Where(fa => fa.FlightId == flightId && fa.UserGroup.User.Role == "Crew")
                .ToListAsync();

            return crewAssignments.Select(fa => fa.UserGroup.User).ToList();
        }

        public async Task<List<User>> GetAllMembersForFlight(int flightId)
        {
            var assignments = await _dbContext.FlightAssignments
                .Include(fa => fa.UserGroup)
                .ThenInclude(ug => ug.User)
                .Where(fa => fa.FlightId == flightId)
                .ToListAsync();

            return assignments.Select(fa => fa.UserGroup.User).ToList();
        }
    }
}
