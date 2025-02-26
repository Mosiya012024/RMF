using MongoDB.Driver;
using RoomMateFinderApplication.Models;

namespace RoomMateFinderApplication.Services
{
    public class AddressDuplicateData
    {

        public AddressDuplicateData()
        {

        }

        public List<Address> getDuplicateAddresses()
        {
            List<Address> allAddresses = new List<Address>()
            {
                // Andhra Pradesh
                new Address { Area = "MVP Colony", City = "Visakhapatnam", State = "Andhra Pradesh" },
                new Address { Area = "Benz Circle", City = "Vijayawada", State = "Andhra Pradesh" },
                new Address { Area = "MG Road", City = "Guntur", State = "Andhra Pradesh" },
                new Address { Area = "Alipiri Road", City = "Tirupati", State = "Andhra Pradesh" },
                new Address { Area = "Roza Street", City = "Kurnool", State = "Andhra Pradesh" },

                // Karnataka
                new Address { Area = "Indiranagar", City = "Bangalore", State = "Karnataka" },
                new Address { Area = "Marathahalli", City = "Benagluru", State = "Karnataka" },
                new Address { Area = "Sector 7", City = "Mysore", State = "Karnataka" },
                new Address { Area = "Station Road", City = "Hubli", State = "Karnataka" },

                // Kerala
                new Address { Area = "Fort Kochi", City = "Kochi", State = "Kerala" },
                new Address { Area = "MG Road", City = "Trivandrum", State = "Kerala" },
                new Address { Area = "Calicut Beach", City = "Calicut", State = "Kerala" },
                new Address { Area = "Thampanoor", City = "Thiruvananthapuram", State = "Kerala" },

                // Maharashtra
                new Address { Area = "Marine Drive", City = "Mumbai", State = "Maharashtra" },
                new Address { Area = "Koregaon Park", City = "Pune", State = "Maharashtra" },
                new Address { Area = "Civil Lines", City = "Nagpur", State = "Maharashtra" },
                new Address { Area = "Shivaji Nagar", City = "Nashik", State = "Maharashtra" },

                // Tamil Nadu
                new Address { Area = "Marina Beach", City = "Chennai", State = "Tamil Nadu" },
                new Address { Area = "Race Course", City = "Coimbatore", State = "Tamil Nadu" },
                new Address { Area = "West Gate", City = "Madurai", State = "Tamil Nadu" },
                new Address { Area = "Srirangam", City = "Trichy", State = "Tamil Nadu" },

            // Delhi
                new Address { Area = "Connaught Place", City = "Gurugram", State = "Delhi" },
                new Address { Area = "Greater Kailash", City = "NCR", State = "Delhi" },
                new Address { Area = "Karol Bagh", City = "Gurgaon", State = "Delhi" },

                // Telangana
                new Address { Area = "Banjara Hills", City = "Hyderabad", State = "Telangana" },
                new Address { Area = "Secunderabad", City = "Secundrabad", State = "Telangana" },
                new Address { Area = "Begumpet", City = "Sanga Reddy", State = "Telangana" },

                new Address { Area = "Dalgate", City = "Srinagar", State = "Jammu and Kashmir" },
                new Address { Area = "Boulevard", City = "Gulmarg", State = "Jammu and Kashmir" },
                new Address { Area = "The Bund",  City = "Pahalgam", State = "Jammu and Kashmir" },

            };
            return allAddresses;
        }
    }
}
