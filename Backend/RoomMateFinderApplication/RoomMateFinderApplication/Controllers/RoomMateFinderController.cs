//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using RoomMateFinderApplication.Enums;
//using RoomMateFinderApplication.Models;
//using RoomMateFinderApplication.Services;
//using System.Diagnostics.Tracing;

//namespace RoomMateFinderApplication.Controllers
//{
    
//    [Route("api/[controller]")]
//    [ApiController]
//    public class RoomMateFinderController : Controller
//    {
//        private readonly IRoomMateFinderManager rmf_manager;
//        public RoomMateFinderController(IRoomMateFinderManager rmf_manager) {
//            this.rmf_manager = rmf_manager;
//        }

//        [Authorize]
//        [HttpGet]
//        [Route("/get/allRooms")]
//        public async Task<IActionResult> GetAllRoomMateFinders(int offset, int limit, string? filter = null,string? orderBy = null)
        
        
//        {
//            //returns all the Rooms.
//            ApiResponseDto result = new ApiResponseDto();
//            try
//            {
//                var allData = await this.rmf_manager.GetAllRooms(filter,orderBy);
//                result.Data = allData.Skip(offset).Take(limit);
//                result.Count = allData.Count;
//                result.Code = ApiResponseCode.Success;
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                List<string> errors = new List<string>();
//                errors!.Add(ex.Message);
//                result.Code = ApiResponseCode.Error;
//                result.Error = errors;
//                return Ok(result);
//            }
//        }

//        [HttpPost]
//        [Route("/post/CreateRoom")]
//        public async Task<IActionResult> PostRoom(RoomDetails roomDetails)
//        {
//            //adds a room and returns all the rooms
//            ApiResponseDto result = new ApiResponseDto();
            
//            try
//            {
//                var allData = await this.rmf_manager.PostRoom(roomDetails);
//                result.Data = allData;
//                result.Count = 1;
//                result.Code = ApiResponseCode.Success;
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                List<string> errors = new List<string>();
//                errors!.Add(ex.Message);
//                result.Code = ApiResponseCode.Error;
//                result.Error = errors;
//                return BadRequest(result);
//            }
            
//        }

//        [HttpPost]
//        [Route("/post/CreateMultipleRooms")]
//        public async Task<IActionResult> PostMultipleRooms(int number)
//        {
//            //this is a testing api, which posts the room based on number and returns all the rooms
//            ApiResponseDto result = new ApiResponseDto();
            
//            try
//            {
//                var allRooms = await this.rmf_manager.PostMultipleRooms(number);
//                result.Data = allRooms;
//                result.Count = allRooms.Count;
//                result.Code = ApiResponseCode.Success;
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                List<string> errors = new List<string>();
//                errors!.Add(ex.Message);
//                result.Code = ApiResponseCode.Error;
//                result.Error = errors;
//                return BadRequest(result);
//            }
//        }

//        [HttpDelete]
//        [Route("/delete/DeleteMultipleRooms")]
//        public async Task<IActionResult> DeleteMultipleRooms(int number)
//        {
//            //testing api, deletes the rooms based on number and return all the rooms

//            ApiResponseDto result = new ApiResponseDto();

//            try
//            {
//                var allRooms = await this.rmf_manager.DeleteMultipleRooms(number);
//                result.Data = allRooms;
//                result.Count = allRooms.Count;
//                result.Code = ApiResponseCode.Success;
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                List<string> errors = new List<string>();
//                errors!.Add(ex.Message);
//                result.Code = ApiResponseCode.Error;
//                result.Error = errors;
//                return BadRequest(result);
//            }
//        }
//        [Authorize]
//        [HttpGet]
//        [Route("/getRDX")]
//        public async Task<IActionResult> GetRoomDescriptionById(string filterId)
//         {
//            ApiResponseDto result = new ApiResponseDto();
//            try
//            {
//                var roomDescription = await this.rmf_manager.GetRoomDescriptionById(filterId);
//                result.Data = roomDescription;
//                result.Code = ApiResponseCode.Success;
//                result.Count = 1;
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                List<string> errors = new List<string>();
//                errors!.Add(ex.Message);
//                result.Code = ApiResponseCode.Error;
//                result.Error = errors;
//                return BadRequest(result);
//            }
//        }

//        [Authorize]
//        [HttpPost]
//        [Route("/post/CreateRoomCombinedDetails")]
//        public async Task<IActionResult> PostRoomCombinedDetails(RoomCombinedDetails roomCombinedDetails)
//        {
//            //adds a room and returns all the rooms
//            ApiResponseDto result = new ApiResponseDto();

//            try
//            {
//                this.rmf_manager.PostRoomCombinedDetails(roomCombinedDetails);
//                result.Data = true;
//                result.Count = 1;
//                result.Code = ApiResponseCode.Success;
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                List<string> errors = new List<string>();
//                errors!.Add(ex.Message);
//                result.Code = ApiResponseCode.Error;
//                result.Error = errors;
//                return BadRequest(result);
//            }

//        }

//        [Authorize]
//        [HttpPut]
//        [Route("/edit/roomCombinedDetails")]
//        public async Task<IActionResult> EditRoomCombinedDetails(RoomCombinedDetails roomCombinedDetails)
//        {
//            ApiResponseDto result = new ApiResponseDto();
//            try
//            {
//                this.rmf_manager.EditRoomCombinedDetails(roomCombinedDetails);
//                result.Data = true;
//                result.Count = 1;
//                result.Code = ApiResponseCode.Success;
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                List<string> errors = new List<string>();
//                errors!.Add(ex.Message);
//                result.Code = ApiResponseCode.Error;
//                result.Error = errors;
//                return BadRequest(result);

//            }
//        }

//        [Authorize]
//        [HttpDelete]
//        [Route("/delete/roomCombinedDetails")]
//        public async Task<IActionResult> DeleteRoomCombinedDetails(string filterId)
//        {
//            ApiResponseDto result = new ApiResponseDto();
//            try
//            {
//                this.rmf_manager.DeleteRoomCombinedDetails(filterId);
//                result.Data = true;
//                result.Count = 1;
//                result.Code = ApiResponseCode.Success;
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                List<string> errors = new List<string>();
//                errors!.Add(ex.Message);
//                result.Code = ApiResponseCode.Error;
//                result.Error = errors;
//                return BadRequest(result);

//            }
//        }

//    }
//}
