namespace Azure_Room_Mate_Finder.Model
{
    public class RoomDetails
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; }

        public Address Address { get; set; }
        
        public Requirement Requirement { get; set; }
        
        public int Amount { get; set; }
        
        public string Status { get; set; }

        public string Identity { get; set; }

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

        public List<string> AssignedTo {  get; set; }
    }
}
