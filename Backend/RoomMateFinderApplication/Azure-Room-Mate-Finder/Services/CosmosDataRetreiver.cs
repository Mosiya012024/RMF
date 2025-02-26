using Azure_Room_Mate_Finder.Model;
using Azure_Room_Mate_Finder.Repository;

namespace Azure_Room_Mate_Finder.Services
{
    public class CosmosDataRetreiver
    {
        private readonly IServiceProvider serviceProvider;
        
        public CosmosDataRetreiver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<List<TEntity>> GetAllAsync<TEntity>() where TEntity : class
        {
            using var scope = this.serviceProvider.CreateScope();
            var RoomDetailsRepository = (ICosmosDbRepository<TEntity>)scope.ServiceProvider.GetService(typeof(ICosmosDbRepository<TEntity>));
            var allRoomDetails = await RoomDetailsRepository.FindAllAsync(null);
            return allRoomDetails;
        }

    }
}
