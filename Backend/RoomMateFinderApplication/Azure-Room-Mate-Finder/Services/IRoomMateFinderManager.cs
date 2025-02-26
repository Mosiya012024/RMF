using Azure_Room_Mate_Finder.Model;
using System.Linq.Expressions;

namespace Azure_Room_Mate_Finder.Services
{
    public interface IRoomMateFinderManager
    {
        public Task<List<RoomDetails>> GetAllRooms(string filter,string orderBy);

        public Task<RoomDetails> PostRoom(RoomDetails roomDetails);

        public Task<List<RoomDescription>> PostRoomCombinedDetails(RoomCombinedDetails roomCombinedDetails);

        public Task<Boolean> EditRoomCombinedDetails(RoomCombinedDetails roomCombinedDetails);

        public Task<List<RoomDetails>> PostMultipleRooms(int number);

        public Task<List<RoomDetails>> DeleteMultipleRooms(int number);

        public Task<RoomDescription> GetRoomDescriptionById(string filterId);

        public Task<Boolean> DeleteRoomCombinedDetails(string filterId);

        public Task<List<string>> PostRoomImages(List<IFormFile> file, string folderId);

        public Task<List<ImageType>> GetRoomImages(string folderId);

        public Task<Boolean> MatchRoomRoomMate(string ID, string fromUser);

        public Task<List<string>> GetMatchedRoomsOrRoomMates(string currentUserName);
    }
}
