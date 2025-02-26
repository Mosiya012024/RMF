namespace ParkingLott.Models2
{
    public class Book
    {
        public Guid Book_Id { get; set; }
        public string Title { get; set; }

        public string Authors { get; set; }

        public string Publishers { get; set; }

        public Guid? BookTakenBy { get; set; } 

        public List<Guid> BookCopyId { get; set; }
    }
}
