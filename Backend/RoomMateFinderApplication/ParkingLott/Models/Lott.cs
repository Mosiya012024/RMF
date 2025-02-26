using ParkingLott.Enums;

namespace ParkingLott.Models
{
    public class Lott
    {
        public Guid  Lott_Id { get; set; }

        public int floor_number { get; set; }

        public Lott_Slots Lott_Slots { get; set; }

        public Lott()
        {
            Lott_Id = Guid.NewGuid();
            Lott_Slots = new Lott_Slots()
            {
                Lott_Slot_Id = Lott_Id,
            };
        }
    }
}
