using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Azure_Room_Mate_Finder.Enums;
using Azure_Room_Mate_Finder.Model;
using Azure_Room_Mate_Finder.Repository;
using Azure_Room_Mate_Finder.Services;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.AspNetCore.Http;
using OtpNet;

namespace Azure_Room_Mate_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ICosmosDbRepository<User> userRepository;
        private readonly IUserManager userManager;
        private readonly JwtService _jwtService;
        private readonly EmailService emailService;
        public UserController(ICosmosDbRepository<User> userRepository, IUserManager userManager, JwtService jwtService, EmailService emailService = null)
        {
            this.userRepository = userRepository;
            this.userManager = userManager;
            _jwtService = jwtService;
            this.emailService = emailService;
        }

        [HttpGet]
        [Route("/get/users")]
        public async Task<IActionResult> getAllUsers()
        {
            ApiResponseDto result = new ApiResponseDto();
            try
            {
                var allUsers = await this.userRepository.FindAllAsync(null);
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

        //[HttpPost]
        //[Route("/post/user/")]
        //public async Task<IActionResult> CreateUser([FromBody] User user)
        //{
        //    ApiResponseDto result = new ApiResponseDto();

        //    try
        //    {
        //        var token = Guid.NewGuid().ToString();
        //        user.Token = token;
        //        await this.userManager.CreateUser(user);

        //        var confirmationLink = Url.ActionLink("ConfirmEmail", "User", new { token });
        //        await this.emailService.SendEmailAsync(user.Email, "Confirm your account by click", confirmationLink);
        //        result.Data = user;
        //        result.Code = ApiResponseCode.Success;
        //        result.Count = 1;
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        List<string> errors = new List<string>();
        //        errors!.Add(ex.Message);
        //        result.Error = errors;
        //        result.Code = ApiResponseCode.Error;
        //        return BadRequest(result);
        //    }
        //}

        //[HttpGet]
        //[Route("/get/confirm-mail")]
        //public async Task<IActionResult> ConfirmEmail(string token)
        //{
        //    ApiResponseDto result = new ApiResponseDto();
        //    try
        //    {
        //        var users = await this.userRepository.FindAllAsync(x => x.Token == token);
        //        var currentUser = users[0];
        //        currentUser.isConfirmed = true;
        //        await this.userRepository.UpdateAsync(currentUser);
        //        result.Data = currentUser;
        //        result.Code = ApiResponseCode.Success;
        //        result.Token = token;
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        List<string> errors = new List<string>();
        //        errors!.Add(ex.Message);
        //        result.Error = errors;
        //        result.Data = false;
        //        result.Code = ApiResponseCode.Error;
        //        return BadRequest(result);
        //    }

        //}

        [HttpPost]
        [Route("/post/user/")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            ApiResponseDto result = new ApiResponseDto();

            try
            {
                var token = Guid.NewGuid().ToString();
                user.Token = token;
                await this.userManager.CreateUser(user);

                var frontendUrl = "http://localhost:3000/confirm-email";
                var confirmationLink = $"{frontendUrl}/email/token/{token}";
                await this.emailService.SendEmailAsync(user.Email, "Confirm your account by click", confirmationLink);
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

        [HttpGet]
        [Route("/get/confirm-mail")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            ApiResponseDto result = new ApiResponseDto();
            try
            {
                var users = await this.userRepository.FindAllAsync(x => x.Token == token);
                var currentUser = users[0];
                currentUser.isConfirmed = true;
                await this.userRepository.UpdateAsync(currentUser);
                result.Data = currentUser;
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

        //[HttpPost]
        //[Route("/validate/user")]

        ////basically a login API using email Id and Password
        //public async Task<IActionResult> ValidateUserLogin([FromBody] LoginDto login)
        //{
        //    ApiResponseDto result = new ApiResponseDto();
        //    var allUsers = await this.userRepository.FindAllAsync(null);
        //    try
        //    {
        //        var user =  await this.userManager.ValidateUserLogin(allUsers,login);
        //        var token = _jwtService.GenerateToken(user.Name,user.Usertype);
        //        result.Data = user;
        //        result.Code = ApiResponseCode.Success;
        //        result.Token = token;
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        List<string> errors = new List<string>();
        //        errors!.Add(ex.Message);
        //        result.Error = errors;
        //        result.Data = false;
        //        result.Code = ApiResponseCode.Error;
        //        return BadRequest(result);

        //    }
        //}

        [HttpPost]
        [Route("/validateandsendotp/user")]

        //basically a login API using email Id and Password
        public async Task<IActionResult> ValidateUserLoginAndSendOTP([FromBody] LoginDto login)
        {
            ApiResponseDto result = new ApiResponseDto();
            var allUsers = await this.userRepository.FindAllAsync(null);
            try
            {
                var Secret = await this.userManager.ValidateUserLoginAndSendOTP(allUsers, login);
                
                result.Data = Secret;
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

        [HttpPost]
        [Route("/validateuserAndGenerateQR/user")]
        public async Task<IActionResult> ValidateUserAndGenerateQR([FromBody] LoginDto login)
        {
            ApiResponseDto result = new ApiResponseDto();
            var allUsers = await this.userRepository.FindAllAsync(null);
            try
            {
                var Secret = await this.userManager.ValidateUserAndGenerateQRCode(allUsers, login);

                result.Data = Secret;
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

        [HttpPost]
        [Route("/validate/user")]
        public async Task<IActionResult> VerifyOtpAndLogin([FromBody] OtpRequest request)
        {
            ApiResponseDto result = new ApiResponseDto();
            
            try
            {
                var user = await this.userRepository.FindAllAsync(x => x.Email == request.UserId); // Retrieve user's info from the database
                var userSecret = user[0].MfaSecret; // Get the stored TOTP secret

                var totp = new Totp(Base32Encoding.ToBytes(userSecret)); // Create TOTP instance
                bool isValid = totp.VerifyTotp(request.OtpCode, out long timeStepMatched, new VerificationWindow(1, 1)); // Allow minor clock drift
                if (isValid)
                {
                    var token = _jwtService.GenerateToken(user[0].Name, user[0].Usertype);
                    result.Data = user[0];
                    result.Code = ApiResponseCode.Success;
                    result.Token = token;
                    return Ok(result);
                }
                else
                {
                    return Unauthorized(new { Message = "Invalid OTP" });
                }
                
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
