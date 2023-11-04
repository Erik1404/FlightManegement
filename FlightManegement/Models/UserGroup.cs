using System.ComponentModel.DataAnnotations;

namespace FlightManegement.Models
{
    public class UserGroup
    {
        [Key]
        public int UserGroupId { get; set; }

        [Required(ErrorMessage = "UserId là trường bắt buộc")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required(ErrorMessage = "GroupId là trường bắt buộc")]
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
