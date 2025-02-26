using Azure_Room_Mate_Finder.Model;
using Azure_Room_Mate_Finder.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Azure_Room_Mate_Finder.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AzureRoomMateFinderController : Controller
    {
        private readonly ICosmosDbRepository<RoomDetails> csmsRepository;
        private readonly RMFDBContext rMFDBContext;

        public AzureRoomMateFinderController(ICosmosDbRepository<RoomDetails> csmsRepository, RMFDBContext rMFDBContext)
        {
            this.csmsRepository = csmsRepository;
            this.rMFDBContext = rMFDBContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var totalRooms = await this.csmsRepository.FindAllAsync(null);
            //var room1 = new RoomDetails()
            //{
            //    Id = Guid.NewGuid().ToString(),
            //    Name = "Mosiya",
            //    Address = "Jhannum",
            //    Amount = 11232,
            //    Identity = "Room Mate Finder",
            //    Requirement = "21 BHK",
            //    Status = "Vacant",

            //};
            //this.rMFDBContext.RoomDetails.Add(room1);
            //this.rMFDBContext.SaveChanges();
            //var allRooms = rMFDBContext.RoomDetails.ToList();
            return Ok(totalRooms);
        }

        [HttpPost]
        public async Task<IActionResult> Post(RoomDetails entityToPost)
        {
            var allRooms = await this.csmsRepository.PostAsync(entityToPost);
            return Ok(allRooms);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAllAsync()
        {
            //DbSet<RoomDetails> roomDetails = this.rMFDBContext.Set<RoomDetails>();
            //var entity = roomDetails.FirstOrDefault(x=>x.Name == name);
            //if(entity != null)
            //{
            //    entity.Name = changeName;
            //    roomDetails.Update(entity);
            //    this.rMFDBContext.SaveChanges();
            //}
            var entity = this.rMFDBContext.Set<RoomDetails>().FirstOrDefault(x => x.Name == "Mosiya");
            entity.Name = "Gadhi Daash";
            var updatedRoom = await this.csmsRepository.UpdateAsync(entity);
            return Ok(updatedRoom);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAllAsync(RoomDetails entityToDelete)
        {
            //DbSet<RoomDetails> roomDetails = this.rMFDBContext.Set<RoomDetails>();
            //var entity = roomDetails.FirstOrDefault(x => x.Name == name);
            //if (entity != null)
            //{

            //    roomDetails.Remove(entity);
            //    this.rMFDBContext.SaveChanges();
            //}

            var allRooms = await this.csmsRepository.DeleteAsync(entityToDelete);

            return Ok(allRooms);
        }


    }
}
