using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomMateFinderApplication.Enums;
using RoomMateFinderApplication.Models;
using RoomMateFinderApplication.Repositories;
using RoomMateFinderApplication.Services;

namespace RoomMateFinderApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IRepository<User> userRepository;
        private readonly IUserManager userManager;
        private readonly JwtService _jwtService;
        public UserController(IRepository<User> userRepository, IUserManager userManager, JwtService jwtService) {
            this.userRepository = userRepository;
            this.userManager = userManager;
            _jwtService = jwtService;
        }

        [HttpGet]
        [Route("/get/users")]
        public async Task<IActionResult> getAllUsers()
        {
            ApiResponseDto result = new ApiResponseDto();
            try
            {
                var allUsers = await this.userRepository.FindAllAsync();
                result.Data = allUsers;
                result.Code = ApiResponseCode.Success;
                result.Count = allUsers.Count;
                return Ok(result);
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Error = errors;
                result.Code = ApiResponseCode.Error;
                return BadRequest(result);
            }
        }

        [HttpPost]
        [Route("/post/user/")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            ApiResponseDto result = new ApiResponseDto();
           
            try
            {
                await this.userManager.CreateUser(user);
                result.Data = user;
                result.Code = ApiResponseCode.Success;
                result.Count = 1;
                return Ok(result);
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Error = errors;
                result.Code = ApiResponseCode.Error;
                return BadRequest(result);
            }
        }

        [HttpPost]
        [Route("/validate/user")]

        //basically a login API using email Id and Password
        public async Task<IActionResult> ValidateUserLogin([FromBody] LoginDto login)
        {
            ApiResponseDto result = new ApiResponseDto();
            var allUsers = await this.userRepository.FindAllAsync();
            try
            {
                var user =  await this.userManager.ValidateUserLogin(allUsers,login);
                var token = _jwtService.GenerateToken(login.Email);
                result.Data = user;
                result.Code = ApiResponseCode.Success;
                result.Token = token;
                return Ok(result);
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Error = errors;
                result.Data = false;
                result.Code = ApiResponseCode.Error;
                return BadRequest(result);

            }
        }

        [Authorize]
        [HttpDelete]
        [Route("/delete/user")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            ApiResponseDto result = new ApiResponseDto();
            try
            {
                var user = await this.userManager.DeleteUser(email);
                result.Code = ApiResponseCode.Success;
                result.Data = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Error = errors;
                result.Data = false;
                result.Code = ApiResponseCode.Error;
                return BadRequest(result);
            }
        }

        [Authorize]
        [HttpPut]
        [Route("/put/user")]
        public async Task<IActionResult> ModifyUser(User user)
        {
            ApiResponseDto result = new ApiResponseDto();
            try
            {
                result.Data = await this.userManager.ModifyUser(user);
                result.Code = ApiResponseCode.Success;
                return Ok(result);
                
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors!.Add(ex.Message);
                result.Error = errors;
                result.Data = false;
                result.Code = ApiResponseCode.Error;
                return BadRequest(result);
            }
        }

    }
}
