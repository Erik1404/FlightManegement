using System.ComponentModel.DataAnnotations;

namespace FlightManegement.Models
{
    public class FlightAssignment
    {

        [Key]
        public int FlightAssignmentId { get; set; }

        [Required]
        public int UserGroupId { get; set; } 
        public UserGroup UserGroup { get; set; }

       
        public int FlightId { get; set; }
        public Flight Flight { get; set; }
    }
}
/* FlightAssignment được tạo với mục đích 
 Mỗi Flight có thể có tối đa hai User với UserGroup tương ứng với vai trò pilot. -- Cơ trưởng cơ phó
 Mỗi Flight có thể có tối đa năm User với UserGroup tương ứng với vai trò crew.*/