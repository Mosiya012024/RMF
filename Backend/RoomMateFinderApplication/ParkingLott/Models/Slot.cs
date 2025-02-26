using ParkingLott.Enums;

namespace ParkingLott.Models
{
    public class Slot
    {
        public Guid Slot_Id { get; set; }

        public LottType Slot_Type { get; set; }

        public int slot_number { get; set; }

        public Guid? Vehicle_Number { get; set; }    

        public Slot_Status Status { get; set; }

        public DateTime? InTime { get; set; }

        public DateTime? OutTime { get; set; }
    }
}
