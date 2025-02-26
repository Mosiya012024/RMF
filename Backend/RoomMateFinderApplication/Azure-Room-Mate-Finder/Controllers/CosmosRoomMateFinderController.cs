using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure_Room_Mate_Finder.Enums;
using Azure_Room_Mate_Finder.Model;
using Azure_Room_Mate_Finder.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.Identity.Client;
using System.Text.Json;

namespace Azure_Room_Mate_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CosmosRoomMateFinderController : Controller
    {
        private readonly IRoomMateFinderManager rmf_manager;
        private readonly ILogger<CosmosRoomMateFinderController> logger;
        private readonly IConfiguration _configuration;

        public CosmosRoomMateFinderController(IRoomMateFinderManager rmf_manager, ILogger<CosmosRoomMateFinderController> logger,IConfiguration configuration)
        {
            this.rmf_manager = rmf_manager;
            this.logger = logger;
            _configuration = configuration;
            var abc = _configuration.GetValue<string>("Name");
            Console.WriteLine(abc);
            var abcd = _configuration.GetValue<string>("AppConfiguration:ConnectionString");
            Console.WriteLine(abcd);
        }

        //[Authorize(Roles ="Room Mate Finder")]
        [Authorize(Policy = "UserType")]
        [HttpGet]
        [Route("/get/allRooms")]
        public async Task<IActionResult> GetAllRoomMateFinders(int offset, int limit, string? filter = null, string? orderBy = null)
        {
            //var abc = _configuration["RedisCacheConnectionString"];
            //var bcd = _configuration["RMFRedisCache"];

            //var keyVaultURI = "https://rmf-app-key-vault.vault.azure.net/";
            //var Client = new SecretClient(new Uri(keyVaultURI), new DefaultAzureCredential());
            //try
            //{
            //    KeyVaultSecret keyVaultSecret = await Client.GetSecretAsync("RedisCacheConnectionString");
            //}
            //catch(Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}

            //returns all the Rooms.
            logger.LogInformation("Get all rooms apis is invoked with filter {filter} and orderby as {orderBy}", filter, orderBy);
            ApiResponseDto result = new ApiResponseDto();
            try
            {
                var allData = await this.rmf_manager.GetAllRooms(filter, orderBy);
                result.Data = allData.Skip(offset).Take(limit);
                result.Count = allData.Count;
                result.Code = ApiResponseCode.Success;
                logger.LogInformation("get all Rooms responded with {ResponseDto}",result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Code = ApiResponseCode.Error;
                result.Error = errors;
                //LoggerMessage.Define<string, ApiResponseDto>(LogLevel.Information, result, " get all rooms api responded with {ApiResponseDto}");
           
                logger.LogError("get all Rooms failed and responded with {ResponseDto}", JsonSerializer.Serialize(result));
                return Ok(result);
            }
        }

        [HttpPost]
        [Route("/post/CreateRoom")]
        public async Task<IActionResult> PostRoom(RoomDetails roomDetails)
        {
            //adds a room and returns all the rooms
            ApiResponseDto result = new ApiResponseDto();

            try
            {
                var allData = await this.rmf_manager.PostRoom(roomDetails);
                result.Data = allData;
                result.Count = 1;
                result.Code = ApiResponseCode.Success;
                return Ok(result);
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Code = ApiResponseCode.Error;
                result.Error = errors;
                return BadRequest(result);
            }

        }

        [HttpPost]
        [Route("/post/CreateMultipleRooms")]
        public async Task<IActionResult> PostMultipleRooms(int number)
        {

            //this is a testing api, which posts the room based on number and returns all the rooms
            logger.LogInformation("post multiple rooms is invoked with number {number}", number);
            ApiResponseDto result = new ApiResponseDto();

            try
            {
                var allRooms = await this.rmf_manager.PostMultipleRooms(number);
                result.Data = allRooms;
                result.Count = allRooms.Count;
                result.Code = ApiResponseCode.Success;
                logger.LogInformation("post multiple Rooms responded with {ResponseDto}", result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                
                errors!.Add(ex.Message);
                result.Code = ApiResponseCode.Error;
                result.Error = errors;
                logger.LogError("get all Rooms failed and responded with {ResponseDto}", JsonSerializer.Serialize(result));
                return BadRequest(result);
            }
        }

        [HttpDelete]
        [Route("/delete/DeleteMultipleRooms")]
        public async Task<IActionResult> DeleteMultipleRooms(int number)
        {
            //testing api, deletes the rooms based on number and return all the rooms

            ApiResponseDto result = new ApiResponseDto();

            try
            {
                var allRooms = await this.rmf_manager.DeleteMultipleRooms(number);
                result.Data = allRooms;
                result.Count = allRooms.Count;
                result.Code = ApiResponseCode.Success;
                return Ok(result);
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Code = ApiResponseCode.Error;
                result.Error = errors;
                return BadRequest(result);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("/getRDX")]
        public async Task<IActionResult> GetRoomDescriptionById(string filterId)
        {
            ApiResponseDto result = new ApiResponseDto();
            try
            {
                var roomDescription = await this.rmf_manager.GetRoomDescriptionById(filterId);
                result.Data = roomDescription;
                result.Code = ApiResponseCode.Success;
                result.Count = 1;
                logger.LogInformation("get Room Description by ID responded with {ResponseDto}", result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Code = ApiResponseCode.Error;
                result.Error = errors;
                logger.LogError("get Room Description ID failed and responded with {ResponseDto}", JsonSerializer.Serialize(result));
                return BadRequest(result);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("/post/CreateRoomCombinedDetails")]
        public async Task<IActionResult> PostRoomCombinedDetails(RoomCombinedDetails roomCombinedDetails)
        {
            //adds a room and returns all the rooms
            logger.LogInformation("post room description and detail is invoked with number");
            ApiResponseDto result = new ApiResponseDto();

            try
            {
                var abcd = await this.rmf_manager.PostRoomCombinedDetails(roomCombinedDetails);
                result.Data = true;
                result.Count = 1;
                result.Code = ApiResponseCode.Success;
                logger.LogInformation("post Room Description and Details responded with {ResponseDto}", result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Code = ApiResponseCode.Error;
                result.Error = errors;
                logger.LogError("post Room Description and Details failed and responded with {ResponseDto}", JsonSerializer.Serialize(result));
                return BadRequest(result);
            }

        }

        [Authorize]
        [HttpPut]
        [Route("/edit/roomCombinedDetails")]
        public async Task<IActionResult> EditRoomCombinedDetails(RoomCombinedDetails roomCombinedDetails)
        {
            ApiResponseDto result = new ApiResponseDto();
            logger.LogInformation("edit room description and detail is invoked with number");
            try
            {
                await this.rmf_manager.EditRoomCombinedDetails(roomCombinedDetails);
                result.Data = true;
                result.Count = 1;
                result.Code = ApiResponseCode.Success;
                logger.LogInformation("post Room Description and Details responded with {ResponseDto}", result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Code = ApiResponseCode.Error;
                result.Error = errors;
                logger.LogError("post Room Description and Details failed and responded with {ResponseDto}", JsonSerializer.Serialize(result));
                return BadRequest(result);

            }
        }

        [Authorize]
        [HttpDelete]
        [Route("/delete/roomCombinedDetails")]
        public async Task<IActionResult> DeleteRoomCombinedDetails(string filterId)
        {
            ApiResponseDto result = new ApiResponseDto();
            try
            {
                await this.rmf_manager.DeleteRoomCombinedDetails(filterId);
                result.Data = true;
                result.Count = 1;
                result.Code = ApiResponseCode.Success;
                return Ok(result);
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Code = ApiResponseCode.Error;
                result.Error = errors;
                return BadRequest(result);

            }
        }

        //[Authorize]
        [HttpPost]
        [Route("/upload/room/images")]
        public async Task<IActionResult> PostRoomImages(string folderId, List<IFormFile> file)
        {
            ApiResponseDto result = new ApiResponseDto();
            try
            {
                var blobUrl = await this.rmf_manager.PostRoomImages(file,folderId);
                result.Data = blobUrl;
                result.Count = 1;
                result.Code = ApiResponseCode.Success;
                return Ok(result);
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Code = ApiResponseCode.Error;
                result.Error = errors;
                return BadRequest(result);

            }
        }

        [Authorize]
        [HttpGet]
        [Route("/get/room/images")]
        public async Task<IActionResult> GetRoomImages(string folderId)
        {
            ApiResponseDto result = new ApiResponseDto();
            try
            {
                List<ImageType> allImages = await this.rmf_manager.GetRoomImages(folderId);
                result.Data = allImages;
                result.Count = 1;
                result.Code = ApiResponseCode.Success;
                return Ok(result);
                
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Code = ApiResponseCode.Error;
                result.Error = errors;
                return BadRequest(result);

            }

        }


        //[Authorize]
        [HttpGet]
        [Route("/match/user")]
        public async Task<IActionResult> MatchRoomRoomMate(string ID,string fromUser)
        {
            ApiResponseDto result = new ApiResponseDto();
            try
            {
                var check = await this.rmf_manager.MatchRoomRoomMate(ID, fromUser);
                result.Data = check;
                result.Count = 1;
                result.Code = ApiResponseCode.Success;
                return Ok(result);
            }
            catch(Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Code = ApiResponseCode.Error;
                result.Error = errors;
                return BadRequest(result);
            }
        }

        //[Authorize]
        [HttpGet]
        [Route("/get/matched/users")]
        public async Task<IActionResult> GetMatchedRoomsOrRoomMates(string currentUserName)
        {
            ApiResponseDto result = new ApiResponseDto();
            try
            {
                var matchedUsers = await this.rmf_manager.GetMatchedRoomsOrRoomMates(currentUserName);
                result.Data = matchedUsers;
                result.Count = 1;
                result.Code = ApiResponseCode.Success;
                return Ok(result);
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Code = ApiResponseCode.Error;
                result.Error = errors;
                return BadRequest(result);
            }
        }


    }
}
