using FlightManegement.Models;

namespace FlightManegement.Interfaces
{
    public interface IGroupService
    {
        Task<List<Group>> GetAllGroups();
        List<Group> SearchGroup(string keyword);
        Task<Group> AddGroup(string Group_Name, string Group_Desc);
        Task<bool> DeleteGroup(int GroupId);
        Task<bool> UpdateGroup(Group Group);
    }
}
