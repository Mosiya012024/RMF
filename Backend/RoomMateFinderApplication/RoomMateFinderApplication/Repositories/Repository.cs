using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using RoomMateFinderApplication.Exceptions;
using RoomMateFinderApplication.Services;
using System.ComponentModel;

namespace RoomMateFinderApplication.Repositories
{
    public class Repository<TEntity>: IRepository<TEntity> where TEntity : class
    {
        public readonly MongoDbService mongoDbService;
        public Repository(MongoDbService mongoDbService)
        {
            this.mongoDbService = mongoDbService;

        }

        public async Task<List<TEntity>> FindAllAsync()
        {
            var collection = this.mongoDbService.GetCollection<TEntity>(typeof(TEntity).Name);
            var users =  collection.Find(x=>true).ToList();
            return users;
        }

        public async void CreateAsync(TEntity entity)
        {
            var collection = this.mongoDbService.GetCollection<TEntity>(typeof(TEntity).Name);
            collection.InsertOne(entity);
        }

        public async Task<List<TEntity>> CreateMultipleAsync(IList<TEntity> entities)
        {
            var collection = this.mongoDbService.GetCollection<TEntity>(typeof(TEntity).Name);
            collection.InsertMany(entities);            
            return collection.Find(x => true).ToList();
        }

        public async Task<List<TEntity>> DeleteMultipleAsync(int number)
        {
            var collection = this.mongoDbService.GetCollection<TEntity>(typeof(TEntity).Name);
            var allItems = collection.Find(x => true).ToList();
            if(allItems.Count<number)
            {
                //throw new CustomException();
                throw new Exception("Not enough Data to delete");
            }
            else
            {
                while(number > 0)
                {
                    
                    var firstItem = allItems[0];
                    //collection.DeleteOne(x => x.Equals(firstItem));
                    await collection.DeleteOneAsync(x=>x.Equals(firstItem));
                    allItems = collection.Find(x => true).ToList();
                    number--;
                }
            }
            return allItems;
        }

        public async Task<IMongoCollection<TEntity>> getCollection()
        {
            var collection = this.mongoDbService.GetCollection<TEntity>(typeof(TEntity).Name);
            return collection;

        }
    }
}
