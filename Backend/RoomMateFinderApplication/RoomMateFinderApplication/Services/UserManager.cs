using MongoDB.Driver;
using RoomMateFinderApplication.Models;
using RoomMateFinderApplication.Repositories;

namespace RoomMateFinderApplication.Services
{
    public class UserManager:IUserManager
    {
        private readonly IRepository<User> userRepository;

        public UserManager(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task<User> ValidateUserLogin(List<User> allusers, LoginDto login)
        {
            var user = allusers.Find(x=>x.Email == login.Email);
            if(user == null)
            {
                throw new Exception("Email not registered");
            }
            else
            {
                if(user.Password != login.Password)
                {
                    throw new Exception($"Invalid password for user email id {user.Email}");
                }
                else
                {
                    return user;
                }
            }
        }

        public async Task<User> CreateUser(User user) {
            var allUsers = await this.userRepository.FindAllAsync();
            var check = allUsers.Find(x => x.Email == user.Email);
            if (check == null)
            {
                this.userRepository.CreateAsync(user);
                return user;
            }
            else
            {
                throw new Exception("Email already exists");

            }
        }

        public async Task<User> DeleteUser(string email)
        {
            var allUsers = await this.userRepository.FindAllAsync();
            var user = allUsers.Find(x => x.Email == email);
            if (user == null)
            {
                throw new Exception("Email not present in the database to delete");
            }
            else
            {
                IMongoCollection<User> collection = await this.userRepository.getCollection();
                collection.DeleteOne(x => x.Email == email);
                return user;
                
            }
        }

        public async Task<Boolean> ModifyUser(User user)
        {
            IMongoCollection<User> userCollection = await this.userRepository.getCollection();
            var userToModify = await userCollection.FindAsync(x=>x.Id == user.Id);
            try
            {
                await userCollection.ReplaceOneAsync(x => x.Id == user.Id, user); 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }
    }
}
