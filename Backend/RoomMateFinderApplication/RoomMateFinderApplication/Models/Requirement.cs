using MongoDB.Bson.Serialization.Attributes;

namespace RoomMateFinderApplication.Models
{
    public class Requirement
    {
        [BsonElement("size")]
        public string Size { get; set; }
        [BsonElement("vacancy")]
        public int Vacancy { get; set; }
        [BsonElement("gender")]
        public string Gender { get; set; }
        [BsonElement("message")]
        public string Message { get; set; }

    }
}
