using Microsoft.AspNetCore.Mvc;
using ParkingLott.Enum2;
using ParkingLott.Interface;
using ParkingLott.Models2;

namespace ParkingLott.Services
{
    public class LibraryRepository : ILibraryRepository
    {
        public Library lib  = new Library();
        public virtual async Task<Library> CreateLibrary()
        {
            lib.Racks = new List<Rack>();
            lib.Racks.Add(new Rack()
            {
                Rack_Id = Guid.NewGuid(),
                Book_Info = new Book()
                {
                    Book_Id = Guid.NewGuid(),
                    BookTakenBy = null,
                    Title = "ajsd",
                    Authors = "ehwihef",
                    Publishers = "wejhwkejf"
                },
                RStatus = Enum2.RackStatus.Occupied,
                FilledBookId = Guid.NewGuid(),
                
            });
            lib.Library_Id = Guid.NewGuid();
            return lib;
        }

        public async Task<Library> GetLibrary()
        {
            await this.CreateLibrary();
            return lib;
        }

        public async Task<Rack> UpdateLibrary(Guid UserID,Guid BookID)
        {
            await this.CreateLibrary();
            var rack = new Rack();
            foreach (var j in lib.Racks)
            {
                if (j.Book_Info.Book_Id == BookID)
                {
                    //var his = j.RackHistory.First(true);
                    //his.TakenUserId = UserID;
                    j.FilledBookId = UserID;
                    j.RStatus = RackStatus.Available;

                    rack = j;
                    break;
                }
            }
            return rack;
        }

        public void DeleteLibrary()
        {
            this.CreateLibrary();
            lib = new Library();
        }



    }
}
