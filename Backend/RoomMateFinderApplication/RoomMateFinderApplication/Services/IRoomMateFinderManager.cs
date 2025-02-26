using Microsoft.Extensions.Diagnostics.HealthChecks;
using RoomMateFinderApplication.Models;

namespace RoomMateFinderApplication.Services
{
    public interface IRoomMateFinderManager
    {
        public Task<List<RoomDetails>> GetAllRooms(string filter,string orderBy);

        public Task<RoomDetails> PostRoom(RoomDetails roomDetails);

        public void PostRoomCombinedDetails(RoomCombinedDetails roomCombinedDetails);

        public void EditRoomCombinedDetails(RoomCombinedDetails roomCombinedDetails);

        public Task<List<RoomDetails>> PostMultipleRooms(int number);

        public Task<List<RoomDetails>> DeleteMultipleRooms(int number);

        public Task<RoomDescription> GetRoomDescriptionById(string filterId);

        public void DeleteRoomCombinedDetails(string filterId);
    }
}
