namespace ParkingLott.Models2
{
    public class User_Info
    {
        private int? number;
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        //public int LeftOverBooks
        //{
        //    get { return 5; }
        //    set { number = value; }
        //}

        public int LeftOverBooks
        {
            get; set;
        }

        private List<BorrowedBooksHistory>? abcde;
        //public List<BorrowedBooksHistory> BorrowedBooksHistory
        //{
        //    get { return new List<BorrowedBooksHistory>(); }
        //    set { abcde = value; }
        //}

        public List<BorrowedBooksHistory> BorrowedBooksHistory
        {
            get; set;
        }
    }
}
