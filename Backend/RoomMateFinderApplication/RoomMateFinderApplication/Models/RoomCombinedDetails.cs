using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Xml.Linq;

namespace RoomMateFinderApplication.Models
{
    public class RoomCombinedDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; }

        public string RoomType { get; set; }

        [BsonElement("address")]
        public Address Address { get; set; }
        [BsonElement("requirement")]
        public Requirement Requirement { get; set; }

        [BsonElement("amount")]
        public int Amount { get; set; }
        public int Deposit { get; set; }

        public int Maintenance { get; set; }

        public string AmenityType { get; set; }

        public List<string> Amenities { get; set; }

        private DateTime? _postedOn;
        public DateTime PostedOn {
            get { return DateTime.Now; }   
            set { _postedOn = value; }  
        }

        public DateTime AvailableFrom { get; set; }

        public string Status { get; set; }

        public string FlatNumber { get; set; }
    }
}
