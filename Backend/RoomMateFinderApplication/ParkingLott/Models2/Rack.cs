using ParkingLott.Enum2;

namespace ParkingLott.Models2
{
    public class Rack
    {
        public Guid Rack_Id { get; set; }

        public Guid? FilledBookId { get; set; }

        public RackStatus RStatus { get; set; }

        public Book? Book_Info { get; set; }

        public List<Rack_History> RackHistory { get; set; } = new List<Rack_History>();

        public Rack(bool check=false)
        {
            if (check == true)
            {
                FilledBookId = Guid.NewGuid();
                Book_Info = new Book()
                {
                    Book_Id = FilledBookId ?? Guid.NewGuid(),
                    
                };
                RackHistory.Add(new Rack_History()
                {
                    FilledBookId = FilledBookId ?? Guid.NewGuid(),
                });
            }
        }
    }
}
