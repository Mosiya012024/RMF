using MongoDB.Bson.Serialization.Attributes;

namespace RoomMateFinderApplication.Models
{
    public class Address
    {
        [BsonElement("area")]
        public string Area { get; set; }
        [BsonElement("city")]
        public string City { get; set; }
        [BsonElement("state")]
        public string State { get; set; }

        //public Address(string area, string city, string state)
        //{
        //    Area = area;
        //    City = city;
        //    State = state;
        //}
    }
}
