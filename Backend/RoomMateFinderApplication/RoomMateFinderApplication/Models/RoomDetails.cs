using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Security.Principal;
using System.Reflection;

namespace RoomMateFinderApplication.Models
{
    public class RoomDetails
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("address")]
        public Address Address { get; set; }
        [BsonElement("requirement")]
        public Requirement Requirement { get; set; }
        [BsonElement("amount")]
        public int Amount { get; set; }
        [BsonElement("status")]
        public string Status { get; set; }

        private string _identity;
        public string Identity { get; set; }
        // {
            //get => _identity;
            //set
            //{
            //    _identity = value;
            //    Random rand = new Random();
            //    List<string> possibleSize = new List<string>() { "1BHK", "2BHK", "3BHK", "4BHK", "5BHK" };
            //    List<string> possibleOccupancy = new List<string>() { "Single", "Double Shared", "Triple Shared", "Fourple Shared" };
            //    if (_identity == "Room Finder")
            //    {
            //        Requirement.Size = possibleOccupancy[rand.Next(0, possibleOccupancy.Count)];
            //        Requirement.Vacancy = 0;
            //    }
            //    else
            //    {
            //        Requirement.Size = possibleSize[rand.Next(0, possibleSize.Count)];
            //        Requirement.Vacancy = rand.Next(2, 11);
            //    }
            //}
        // }
        public void SetRequirementIdentity()
        {
            
            Random rand = new Random();
            List<string> possibleSize = new List<string>() { "1BHK", "2BHK", "3BHK", "4BHK", "5BHK" };
            List<string> possibleOccupancy = new List<string>() { "Single", "2 - Shared", "3 - Shared", "4 - Shared" };
            List<string> possibleGender = new List<string>() { "Male", "Female", "Any" };
            var shuffeldGender = possibleGender.OrderBy(x => rand.Next()).ToList();
            if (Identity == "Room Finder")
            {
                Requirement.Size = possibleOccupancy[rand.Next(0, possibleOccupancy.Count)];
                Requirement.Vacancy = 0;
                Requirement.Gender = shuffeldGender[0];
                Requirement.Message = "abcdefg";
            }
            else
            {
                Requirement.Size = possibleSize[rand.Next(0, possibleSize.Count)];
                Requirement.Vacancy = rand.Next(2, 11);
                Requirement.Gender = shuffeldGender[0];
                Requirement.Message = "abcdefg";
            }
        }
    }
}
