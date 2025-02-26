using Azure_Room_Mate_Finder.Model;
using Azure_Room_Mate_Finder.Repository;
using Azure_Room_Mate_Finder.Services;
using System.Runtime.Serialization;
using System.Security.Policy;
using OtpNet;
using QRCoder;

namespace Azure_Room_Mate_Finder.Services
{
    public class UserManager:IUserManager
    {
        private readonly ICosmosDbRepository<User> userRepository;
        private readonly EmailService emailService;

        public UserManager(ICosmosDbRepository<User> userRepository, EmailService emailService)
        {
            this.userRepository = userRepository;
            this.emailService = emailService;
        }
        public async Task<User> ValidateUserLogin(List<User> allusers, LoginDto login)
        {

            var user = allusers.Find(x => x.Email == login.Email);

            if (user == null)
            {
                throw new Exception("Email not registered");
            }
            else
            {
                if (user.Password != login.Password)
                {
                    throw new Exception($"Invalid password for user email id {user.Email}");
                }
                else
                {
                    var secret = this.SetupMfa(user);
                    return user;
                }
            }
        }

        public async Task<string> ValidateUserAndGenerateQRCode(List<User> allusers, LoginDto login)
        {
            var user = allusers.Find(x => x.Email == login.Email);

            if (user == null)
            {
                throw new Exception("Email not registered");
            }
            else
            {
                if (user.Password != login.Password)
                {
                    throw new Exception($"Invalid password for user email id {user.Email}");
                }
                else
                {
                    var secret = "";
                    if (user.MfaSecret == "")
                    {
                        secret = await this.GenerateQrSetupMfa(user);
                    }

                    return secret ?? "";
                }
            }

        }

        public async Task<string> GenerateQrSetupMfa(User user)
        {
            var secretKey = KeyGeneration.GenerateRandomKey(20);
            var base32Secret = Base32Encoding.ToString(secretKey);
            user.MfaSecret = base32Secret;
            this.userRepository.UpdateAsync(user);
            var issuer = "Room Mate Finder";
            var otpPathUrl = $"otpauth://totp/{issuer}:{user.Name}?secret={user.MfaSecret}&issuer={issuer}&digits=6";
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(otpPathUrl, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);

            // Convert to Base64 to display on the frontend
            var qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);
            return qrCodeBase64 ;

            
        }

        public async Task<string> ValidateUserLoginAndSendOTP(List<User> allusers, LoginDto login)
        {

            var user = allusers.Find(x => x.Email == login.Email);

            if (user == null)
            {
                throw new Exception("Email not registered");
            }
            else
            {
                if (user.Password != login.Password)
                {
                    throw new Exception($"Invalid password for user email id {user.Email}");
                }
                else
                {
                    var secret = "";
                    if (user.MfaSecret == "")
                    {
                        secret = await this.SetupMfa(user);
                    }
                   
                    return secret ?? "";
                }
            }
        }

        public async Task<string> SetupMfa(User user)
        {
            var secretKey = KeyGeneration.GenerateRandomKey(20);
            var base32Secret = Base32Encoding.ToString(secretKey);
            user.MfaSecret = base32Secret;
            this.userRepository.UpdateAsync(user);
            return base32Secret;
        }

        public async Task<User> CreateUser(User user) {
            var allUsers = await this.userRepository.FindAllAsync(null);
            var check = allUsers.Find(x => x.Email == user.Email);
            if (check == null)
            {
                await this.userRepository.PostAsync(user);
                return user;
            }
            else
            {
                throw new Exception("Email already exists");

            }
        }

        public async Task<User> DeleteUser(string email)
        {
            var allUsers = await this.userRepository.FindAllAsync(null);
            var user = allUsers.Find(x => x.Email == email);
            if (user == null)
            {
                throw new Exception("Email not present in the database to delete");
            }
            else
            {
                IList<User> collection = await this.userRepository.FindAllAsync(x => x.Email == email);
                await this.userRepository.DeleteAsync(user);
                return user;
                
            }
        }

        public async Task<Boolean> ModifyUser(User user)
        {
            //IMongoCollection<User> userCollection = await this.userRepository.getCollection();
            //var userToModify = await userCollection.FindAsync(x=>x.Id == user.Id);
            try
            {
                await this.userRepository.UpdateAsync(user);
                //await userCollection.ReplaceOneAsync(x => x.Id == user.Id, user); 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }
    }
}
