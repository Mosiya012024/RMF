using Azure_Room_Mate_Finder.Enums;

namespace Azure_Room_Mate_Finder.Model
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
