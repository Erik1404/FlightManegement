using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightManegement.Models
{
    public class Flight
    {
        [Key] // Khóa chính
        public int FlightId { get; set; } // ID của chuyến bay

        [Required] // Thuộc tính bắt buộc
        public string FlightName { get; set; } // Tên hãng hàng không

        public string FlightNumber { get; set; }

        public string Depart { get; set; } // khởi hành

        public string Destination { get; set; } // đến nơi

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        [Required] // Thuộc tính bắt buộc
        public FlightStatus Status { get; set; } 
        public string Notes { get; set; } // Ghi chú thêm về chuyến bay

    }

    public enum FlightStatus
    {
        Scheduled, // Dự kiến
        Boarding,  // Đang lên máy bay
        Departed,  // Đã khởi hành
        Cancelled, // Đã hủy
        Arrived,   // Đã đến
        Delayed    // Bị trì hoãn
    }
}
