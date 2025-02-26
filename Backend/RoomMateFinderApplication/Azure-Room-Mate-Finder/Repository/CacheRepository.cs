using Azure_Room_Mate_Finder.Configuration;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System.Linq.Expressions;

namespace Azure_Room_Mate_Finder.Repository
{
    public class CacheRepository<TEntity> : ICacheRepository<TEntity> where TEntity : class
    {
        private readonly IMemoryCache memoryCache;
        private readonly ICosmosDbRepository<TEntity> cosmosDbRepository;
        private readonly IRedisCacheManager redisCacheManager;
        
        public CacheRepository(IMemoryCache memoryCache, ICosmosDbRepository<TEntity> cosmosDbRepository,IRedisCacheManager redisCacheManager)
        {
            this.memoryCache = memoryCache;
            this.cosmosDbRepository = cosmosDbRepository;
            this.redisCacheManager = redisCacheManager;
        }

        public async Task<List<TEntity>> GetAll(string key) 
        {
            try
            {
                if(!string.IsNullOrEmpty(key))
                {
                    var allItemsFromMemory = this.memoryCache.Get<IEnumerable<TEntity>>(typeof(TEntity).Name + "_KEY").ToList();
                    return allItemsFromMemory;
                }
                else
                {
                    var allItemsFromDB = await this.cosmosDbRepository.FindAllAsync(null);
                    
                    this.memoryCache.Set(key,allItemsFromDB);
                    return allItemsFromDB;
                }
            }
            catch(Exception ex) when (ex is RedisConnectionException)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateMemoryCache(string key, TEntity entity, Expression<System.Func<TEntity, bool>> filter)
        {
            try
            {
                var EntityCache = this.memoryCache.Get<List<TEntity>>(key);
                var changedEntity = this.memoryCache.Get<List<TEntity>>(key).AsQueryable().Where(filter).ToList()[0];
                EntityCache.Remove(changedEntity);
                EntityCache.Add(entity);
                this.memoryCache.Set(key, EntityCache);
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public async Task<bool> DeleteMemoryCache(string key, TEntity entity)
        {
            try
            {
                var EntityCache = this.memoryCache.Get<List<TEntity>>(key);
                EntityCache.Remove(entity);
                this.memoryCache.Set(key, EntityCache);

                //deleting entity from redis cache
                List<TEntity> EntityCollection = new List<TEntity>
                {
                    entity
                };
                IReadOnlyCollection<TEntity> ROCTEntity = EntityCollection;
                this.redisCacheManager.UpdateAllAsync<TEntity>(EntityCollection);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
