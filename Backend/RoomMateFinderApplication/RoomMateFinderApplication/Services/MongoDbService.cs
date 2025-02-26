using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RoomMateFinderApplication.Models;
using System.Linq.Expressions;

namespace RoomMateFinderApplication.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase database;
        private readonly IConfiguration configuration;

        public MongoDbService(IConfiguration configuration)
        {
            this.configuration = configuration;
            MongoClient client = new MongoClient(this.configuration.GetValue<String>("MongoDB:ConnectionURI"));
            database = client.GetDatabase(this.configuration.GetValue<String>("MongoDB:DatabaseName"));
        }
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return database.GetCollection<T>(collectionName);
        }

        public async Task<List<RoomDetails>> GetUsersAsync(Expression<Func<RoomDetails, bool>> filter)
        {
            var coll = database.GetCollection<RoomDetails >("RoomDetails");
            return await coll.Find(filter).ToListAsync();
        }
    }
}
