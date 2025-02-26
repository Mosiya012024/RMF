using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Xml.Linq;

namespace RoomMateFinderApplication.Models
{
    public class RoomDescription
    {
        private List<string> _amenities = new List<string>();
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string RoomId { get; set; }

        public string FlatNumber { get; set; }

        public string AmenityType { get; set; }

        public List<string> Amenities {
            get
            {
                Random rand = new Random();
                List<string> allAmenities = new List<string>() { "Sofa", "TV", "Refrigerator", "Fan", "AC", "WashingMachine" };
                if ((AmenityType == "Fully Furnished" || AmenityType == "Semi Furnished") && _amenities.Count == 0)
                {
                    if(AmenityType == "Fully Furnished")
                    {
                        _amenities = allAmenities;
                    }
                    else
                    {
                        int count = rand.Next(1, allAmenities.Count-1);
                        for (int x = 0; x < count; x++)
                        {
                            _amenities.Add(allAmenities[x]);
                        }
                    }
                    
                }
                return _amenities;
               
            }
            set
            {
                _amenities = value;
            }
        }

        public int Deposit {  get; set; }

        public int Maintenance {  get; set; }

        public DateTime PostedOn { get; set; }

        public DateTime AvailableFrom { get; set; }

        public string Gender { get; set; }

        public int Rent { get; set; }

        public string Size {  get; set; }

        public string State { get; set; }
    }
}
