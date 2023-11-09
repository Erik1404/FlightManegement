using FlightManegement.Models;

namespace FlightManegement.Interfaces
{
    public interface IFlightAssignmentService
    {
        Task<bool> AddMemberForFlight(int userGroupId, int flightId);
        Task<bool> RemoveUserFromFlight(int userGroupId, int flightId);
        Task<List<User>> GetPilotsForFlight(int flightId);
        Task<List<User>> GetCrewForFlight(int flightId);
        Task<List<User>> GetAllMembersForFlight(int flightId);
    }
}
