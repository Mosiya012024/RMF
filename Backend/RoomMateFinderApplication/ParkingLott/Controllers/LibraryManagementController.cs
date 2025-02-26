using Microsoft.AspNetCore.Mvc;
using ParkingLott.Enum2;
using ParkingLott.Models2;
using System.Runtime.CompilerServices;

namespace ParkingLott.Controllers
{
    public class LibraryManagementController : Controller
    {
        public static readonly Library lib = new Library();
        public static readonly List<User_Info> users = new List<User_Info>();
        public static readonly List<Book> books = new List<Book>(); 

        [HttpPost]
        [Route("/post/library")]
        public async Task<IActionResult> CreateALibrary(int racks)
        {
            int number = lib.Racks.Count();
            List<Rack> rackList = new List<Rack>();
            while(racks >= 1)
            {
                rackList.Add(new Rack()
                {
                    Rack_Id = Guid.NewGuid(),
                    FilledBookId = null,
                    RStatus = RackStatus.Available,
                    Book_Info = null
                });
                racks--;
            }
            lib.Library_Id = Guid.NewGuid();
            lib.Racks = rackList;
            return Ok(lib);
        }

        [HttpPost]
        [Route("/add/book")]
        public async Task<IActionResult> AddABook([FromBody] Book book)
        {
           var OccupiedRack = lib.Racks.OrderBy(x=>x.Rack_Id).First(x=>x.RStatus ==  RackStatus.Available);
            if(OccupiedRack != null)
            {
                OccupiedRack.FilledBookId = book.Book_Id;
                OccupiedRack.RStatus = RackStatus.Occupied;
                OccupiedRack.RackHistory.Add(new Rack_History()
                {
                    FilledBookId = book.Book_Id,
                });
                OccupiedRack.Book_Info = book;
            }
            return Ok(new { OccupiedRack, lib });
        }

        [HttpDelete]
        [Route("/remove/book")]
        public async Task<IActionResult> RemoveBook(Guid Book_ID)
        {
            foreach(var i in lib.Racks)
            {
                if(Book_ID == i.Book_Info.Book_Id)
                {
                    i.FilledBookId = null;
                    i.RStatus = RackStatus.Available;
                    i.Book_Info = null;
                    break;
                }
            }
            return Ok();
        }

        [HttpPut]
        [Route("/add/user/book")]
        public async Task<IActionResult> AssignUserToBook([FromBody] User_Info user, Guid BookID)
        {
            var rack = new Rack();
            foreach (var j in lib.Racks)
            {
                if(j.Book_Info.Book_Id == BookID)
                {
                    var his = j.RackHistory.First(x => x.FilledBookId == BookID);
                    his.TakenUserId = user.UserId;
                    j.FilledBookId = null;
                    j.RStatus = RackStatus.Available;

                    user.LeftOverBooks -= 1;
                    rack = j;
                    break;
                }
            }
            user.BorrowedBooksHistory.Add(new BorrowedBooksHistory()
            {
                BorrowedBookId = BookID,
                TimeTaken = DateTime.Now,
            });
            users.Add(user);
            return Ok(rack);

        }

        [HttpPut]
        [Route("/return/user/book")]
        public async Task<IActionResult> UserReturnsBook([FromBody] Book book,Guid UserID)
        {
            var assignedRack = lib.Racks.First(x => x.RStatus == RackStatus.Available);
            assignedRack.RStatus = RackStatus.Occupied;
            assignedRack.FilledBookId = book.Book_Id;
            assignedRack.Book_Info = book;
            //assignedRack.RackHistory.Where(x=>x.FilledBookId == ).(new Rack_History()
            //{
            //    FilledBookId = book.Book_Id,
            //});
            users.First(x=>x.UserId == UserID).BorrowedBooksHistory.First(x=>x.BorrowedBookId == book.Book_Id).TimeGiven = DateTime.Now;
            return Ok(new { assignedRack,users });

        }

        [HttpGet]
        [Route("/access/user/HIstory")]
        public async Task<IActionResult> accessUserHistory(Guid UserID)
        {
            var allBookIDS = new List<Guid>();
            foreach (var k in lib.Racks)
            {
                foreach(var y in k.RackHistory)
                {
                    if(y.TakenUserId == UserID)
                    {
                        allBookIDS.Add(y.FilledBookId);
                    }
                }
            }
            return Ok(allBookIDS);

        }

        [HttpPut]
        [Route("/remove/book/copy")]
        public async Task<IActionResult> RemoveBookCopy(Guid BookCopyID)
        {
            var abc = lib.Racks.First(x=>x.FilledBookId == BookCopyID);
            abc.Book_Info = null;
            abc.RStatus = RackStatus.Available;
            return Ok(abc);
        }

        [HttpPut]
        [Route("/remove/book/copy")]
        public async Task<IActionResult> RemoveBookCopy(Guid BookID,Guid UserID)
        {
            var bookCopyIDS = books.First(x => x.Book_Id == BookID).BookCopyIds;
            foreach(var i in lib.Racks)
            {
                if(bookCopyIDS.Contains(i.FilledBookId ?? Guid.NewGuid()))
                {
                    i.RStatus = RackStatus.Available;
                    i.Book_Info = null;
                    i.RackHistory.First(x => x.FilledBookId == BookID).TakenUserId = UserID;
                }
            }
            return Ok(bookCopyIDS);
          
        }




    }
}
