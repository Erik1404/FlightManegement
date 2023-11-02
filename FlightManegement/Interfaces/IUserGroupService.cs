namespace FlightManegement.Interfaces
{
    public interface IUserGroupService
    {
        Task<bool> AddUserToGroupAsync(int userId, int groupId);
        Task<bool> RemoveUserFromGroupAsync(int userId, int groupId);
        Task<List<int>> GetGroupsForUserAsync(int userId);
    }
}
