
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace Azure_Room_Mate_Finder.Model
{
    public class RoomDescription
    {
        private List<string> _amenities = new List<string>();
        public string RoomId { get; set; }
        public string? Id { get; set; }
        public string FlatNumber { get; set; }

        public string AmenityType { get; set; }

        public List<string> Amenities { get; set; }

        //public List<string> Amenities {
        //    get
        //    {
        //        Random rand = new Random();
        //        List<string> allAmenities = new List<string>() { "Sofa", "TV", "Refrigerator", "Fan", "AC", "WashingMachine" };
        //        if ((AmenityType == "Fully Furnished" || AmenityType == "Semi Furnished") && _amenities.Count == 0)
        //        {
        //            if(AmenityType == "Fully Furnished")
        //            {
        //                _amenities = allAmenities;
        //            }
        //            else
        //            {
        //                int count = rand.Next(1, allAmenities.Count-1);
        //                for (int x = 0; x < count; x++)
        //                {
        //                    _amenities.Add(allAmenities[x]);
        //                }
        //            }
                    
        //        }
        //        return _amenities;
               
        //    }
        //    set
        //    {
        //        _amenities = value;
        //    }
        //}

        public void SetAmenities()
        {
            Random rand = new Random();
            List<string> allAmenities = new List<string>() { "Sofa", "TV", "Refrigerator", "Fan", "AC", "WashingMachine" };
            if ((AmenityType == "Fully Furnished" || AmenityType == "Semi Furnished") && _amenities.Count == 0)
            {
                if (AmenityType == "Fully Furnished")
                {
                    Amenities = allAmenities;
                }
                else
                {
                    int count = rand.Next(1, allAmenities.Count - 1);
                    for (int x = 0; x < count; x++)
                    {
                        Amenities.Add(allAmenities[x]);
                    }
                }

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
