using ParkingLott.Enums;

namespace ParkingLott.Models
{
    public class Vehicle
    {
        public Guid Vehicle_Number {  get; set; }

        public LottType Vehiicle_Type { get; set; }

        public string Color { get; set; }
    }
}
