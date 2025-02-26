using RoomMateFinderApplication.Models;

namespace RoomMateFinderApplication.Services
{
    public interface IUserManager
    {
        public Task<User> ValidateUserLogin(List<User> allusers,LoginDto login);

        public Task<User> CreateUser(User user);

        public Task<User> DeleteUser(string email);

        public Task<Boolean> ModifyUser(User user);
    }
}
