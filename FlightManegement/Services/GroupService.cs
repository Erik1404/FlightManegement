using FlightManegement.Data;
using FlightManegement.Interfaces;
using FlightManegement.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightManegement.Services
{
    public class GroupService : IGroupService
    {
        private readonly FlightManagementDbContext _dbContext;

        public GroupService(FlightManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Group>> GetAllGroup()
        {
            return await _dbContext.Groups.ToListAsync();
        }


        public async Task<List<Group>> GetAllGroups()
        {
            return await _dbContext.Groups.ToListAsync();
        }

        public async Task<Group> GetGroupById(int GroupId)
        {
            return await _dbContext.Groups.FindAsync(GroupId);
        }

        public List<Group> SearchGroup(string keyword)
        {

            return _dbContext.Groups
                .Where(s =>
                    s.GroupId.ToString().Contains(keyword) ||
                    s.Group_Name.Contains(keyword))
                .ToList();
        }

        public async Task<Group> AddGroup(string Group_Name,string Group_Desc)
        {

            var newGroup = new Group
            {
                Group_Name = Group_Name,
                Group_Description = Group_Desc,
            };
            _dbContext.Groups.Add(newGroup);
            await _dbContext.SaveChangesAsync();
            return newGroup;
        }

        public async Task<bool> DeleteGroup(int GroupId)
        {
            var Group = await _dbContext.Groups.FindAsync(GroupId);
            if (Group == null)
                return false;
            _dbContext.Groups.Remove(Group);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateGroup(Group Group)
        {
            _dbContext.Entry(Group).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
