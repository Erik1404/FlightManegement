using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlightManegement.Models
{
    public class FlightDoc
    {
        [Key]
        public int FlightDocId { get; set; }

        public int FlightId { get; set; } 

        public virtual Flight Flight { get; set; } 


        public string DocumentName { get; set; } // Tên tài liệu

        public string Type { get; set; } // Loại tài liệu, ví dụ: "Load Summary", "Cargo Manifest"

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public string Creator { get; set; } 

        public double Version { get; set; } 

        public string FilePath { get; set; } // Đường dẫn lưu tệp PDF

    }
}
