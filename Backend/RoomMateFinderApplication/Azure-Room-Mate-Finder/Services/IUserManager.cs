using Azure_Room_Mate_Finder.Model;

namespace Azure_Room_Mate_Finder.Services
{
    public interface IUserManager
    {
        public Task<User> ValidateUserLogin(List<User> allusers,LoginDto login);

        public Task<string> ValidateUserLoginAndSendOTP(List<User> allusers, LoginDto login);

        public Task<string> ValidateUserAndGenerateQRCode(List<User> allusers, LoginDto login);
        public Task<User> CreateUser(User user);

        public Task<User> DeleteUser(string email);

        public Task<Boolean> ModifyUser(User user);
    }
}
