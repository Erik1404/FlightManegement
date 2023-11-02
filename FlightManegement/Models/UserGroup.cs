using System.ComponentModel.DataAnnotations;

namespace FlightManegement.Models
{
    public class UserGroup
    {
        [Key]
        public int UserGroupId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
