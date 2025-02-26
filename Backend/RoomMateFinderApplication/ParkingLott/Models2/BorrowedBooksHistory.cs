namespace ParkingLott.Models2
{
    public class BorrowedBooksHistory
    {
        public Guid BorrowedBookId { get; set; }

        public DateTime? TimeTaken { get; set; }

        public DateTime? TimeGiven { get; set; }
    }
}
