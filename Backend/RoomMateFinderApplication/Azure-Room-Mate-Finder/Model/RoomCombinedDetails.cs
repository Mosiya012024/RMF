
namespace Azure_Room_Mate_Finder.Model
{
    public class RoomCombinedDetails
    {
        
        public string Id { get; set; } = string.Empty;

        
        public string Name { get; set; }

        public string RoomType { get; set; }

        
        public Address Address { get; set; }
        
        public Requirement Requirement { get; set; }

        
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
