using System.ComponentModel.DataAnnotations;

namespace FlightManegement.Models
{
    public class Flight
    {
        [Key]
        public string FlightId { get; set; }
        public string FlightName { get; set; }
        public string FlightBrand { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
