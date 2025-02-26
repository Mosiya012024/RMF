using Microsoft.AspNetCore.Mvc;
using ParkingLott.Enum2;
using ParkingLott.Interface;
using ParkingLott.Models2;
using ParkingLott.Services;

namespace ParkingLott.Controllers
{
    public class LibraryDesignPatternController : Controller
    {
        private readonly ILibraryRepository libraryRepository;


        public LibraryDesignPatternController(ILibraryRepository libraryRepository)
        {
            this.libraryRepository = libraryRepository;
        }

        [HttpPost]
        [Route("/post/library/sss")]
        public async Task<IActionResult> CreateLibrary()
        {
            //var data = await this.lrDupe.CreateLibrary();
            var data = await this.libraryRepository.CreateLibrary();
            return Ok(data);
        }

        [HttpGet]
        [Route("/get/library/sss")]
        public async Task<IActionResult> GetLibrary()
        {
            var data = this.libraryRepository.GetLibrary();
            return Ok(data);
        }

        [HttpPut]
        [Route("/put/library/sss")]
        public async Task<IActionResult> UpdateLibrary(Guid UserID, Guid BookID)
        {
            var data = await this.libraryRepository.UpdateLibrary(UserID, BookID);
            return Ok(data);
        }

        [HttpDelete]
        [Route("/delete/library/sss")]
        public async Task<IActionResult> DeleteLibrary()
        {
            this.libraryRepository.DeleteLibrary();
            return Ok(200);
        }

    }
}
