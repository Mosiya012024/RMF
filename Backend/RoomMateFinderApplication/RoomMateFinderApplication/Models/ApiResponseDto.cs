using RoomMateFinderApplication.Enums;

namespace RoomMateFinderApplication.Models
{
    public class ApiResponseDto
    {
        public dynamic Data { get; set; }
        
        public int Count { get; set; }

        public ApiResponseCode Code { get; set; }

        public List<string> Error { get; set; }

        public string? Token { get; set; }
    }
}
