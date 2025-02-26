using ParkingLott.Enums;

namespace ParkingLott.Models
{
    public class ApiResponseDto
    {
        public dynamic Data { get; set; }

        public List<string> Errors { get; set; }

        public ApiResponseCode Code { get; set; }

        public string Ticket_ID { get; set; }
    }
}
