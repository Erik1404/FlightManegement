using FlightManegement.Data;
using FlightManegement.Interfaces;
using FlightManegement.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightManegement.Services
{
    public class UserGroupService : IUserGroupService
    {
        private readonly FlightManagementDbContext _dbContext;

        public UserGroupService(FlightManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> AddUserToGroupAsync(int userId, int groupId)
        {
            try
            {
                // Check if the user already belongs to the group
                if (_dbContext.UserGroups.Any(ug => ug.UserId == userId && ug.GroupId == groupId))
                {
                    return false; // User is already in the group
                }
                if (userId == 0 || groupId == 0)
                {
                    throw new Exception("UserId không hợp lệ.");
                }

             
                var user = _dbContext.Users.FirstOrDefault(u => u.UserId == userId);
                var group = _dbContext.Groups.FirstOrDefault(g => g.GroupId == groupId);

                // Kiểm tra xem người dùng có role Admin không
                if (user != null && user.Role == "Admin")
                {
                    throw new Exception("Không thể thêm Admin vào nhóm.");
                }
                if (user != null && group != null)
                {
                    user.Role = group.Group_Name; // Update user's role based on the group name
                    user.UserCode = $"{group.Group_Name}{GetNextUserCode(group.Group_Name)}";

                    _dbContext.UserGroups.Add(new UserGroup { UserId = userId, GroupId = groupId });
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                return false; // User or group not found
            }
            catch
            {
                return false; // An error occurred
            }
        }


        public async Task<bool> RemoveUserFromGroupAsync(int userId, int groupId)
        {
            try
            {
                var userGroup = _dbContext.UserGroups.FirstOrDefault(ug => ug.UserId == userId && ug.GroupId == groupId);
                if (userGroup != null)
                {
                    _dbContext.UserGroups.Remove(userGroup);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                return false; // User is not in the group
            }
            catch
            {
                return false; // An error occurred
            }
        }

        public async Task<List<int>> GetGroupsForUserAsync(int userId)
        {
            return _dbContext.UserGroups.Where(ug => ug.UserId == userId).Select(ug => ug.GroupId).ToList();
        }


        private int GetNextUserCode(string group)
        {
            var maxUserCode = _dbContext.Users
                .Where(u => u.UserCode.StartsWith(group))
                .Select(u => u.UserCode)
                .OrderByDescending(u => u)
                .FirstOrDefault();

            if (maxUserCode != null && maxUserCode.Length > group.Length)
            {
                if (int.TryParse(maxUserCode.Substring(group.Length), out var lastNumber))
                {
                    return lastNumber + 1;
                }
            }

            return 1; // If no existing user codes found, start with 1
        }
    }
}
