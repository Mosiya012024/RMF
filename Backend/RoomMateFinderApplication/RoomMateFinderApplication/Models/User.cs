using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RoomMateFinderApplication.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string? Id { get; set; }
        public string Usertype { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
