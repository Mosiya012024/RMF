using Azure_Room_Mate_Finder.Model;
using Azure_Room_Mate_Finder.Services;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace Azure_Room_Mate_Finder.Configuration
{
    public class StartupCache : IStartupCache
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IDatabase redisCache;
        private readonly IConnectionMultiplexer _redis;
        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache;

        public StartupCache(IServiceProvider serviceProvider, IConnectionMultiplexer redis, IConfiguration configuration, IMemoryCache memoryCache) 
        {
            this.memoryCache = memoryCache;
            this.serviceProvider = serviceProvider;
            _redis = redis;
            redisCache = _redis.GetDatabase();
            this.configuration = configuration;
        }

        public async Task InitiateRedisCache()
        {
            await this.LoadCache<RoomDetails>().ConfigureAwait(false);
            await this.LoadCache<RoomDescription>().ConfigureAwait(false);
        }

        public async Task FlushCache()
        {
            var server = _redis.GetServer(this.configuration.GetValue<string>("RedisCache:ConnectionString"));
            await server.FlushDatabaseAsync().ConfigureAwait(false);
            await Task.CompletedTask;
        }

        private void SetKeyValueList(string key,List<string> value)
        {
            var cache = this.redisCache.StringSet(key, JsonConvert.SerializeObject(value));
            
            //checking that the cache is set correctly or not
            var redisDataString = this.redisCache.StringGet(key);
            var redisDataObject = JsonConvert.DeserializeObject<List<string>>(redisDataString!);
            Console.WriteLine(redisDataObject);
        }

        private void SetList(Dictionary<RedisKey,RedisValue> value)
        {
            this.redisCache.StringSet(value.ToArray());
        }

        public async Task LoadCache<TEntity>() where TEntity : class
        {
            //this.redisCache.KeyDelete(typeof(TEntity).Name+"_KEY");
            CosmosDataRetreiver cosmosDataRetriever = new CosmosDataRetreiver(this.serviceProvider);
            List<TEntity> allData = await cosmosDataRetriever.GetAllAsync<TEntity>();
            var allDataIds = new List<string>();
            if (typeof(TEntity).Name == "RoomDetails")
            {
                allDataIds = allData.Where(x => x != null).Select(x => JObject.Parse(JsonConvert.SerializeObject(x))["Id"].ToString()).ToList();
            }
            else
            {
                var DuplicateallDataIds = allData.Where(x => x != null).Select(x => JObject.Parse(JsonConvert.SerializeObject(x))["RoomId"].ToString()).ToList();
                
                allDataIds = DuplicateallDataIds.Select(s => "RoomId|" + s).ToList();
            }
            if (allDataIds != null)
            {
                this.SetKeyValueList(typeof(TEntity).Name.ToString() + "_KEY", allDataIds);
                
                var documents = new Dictionary<RedisKey, RedisValue>();

                foreach (var item in allData)
                {

                    var value = JsonConvert.SerializeObject(item);
                    if (item != null)
                    {
                        var dummyId = JObject.Parse(JsonConvert.SerializeObject(item));
                        var realId = "";
                        if (typeof(TEntity).Name == "RoomDetails")
                        {
                            realId = dummyId["Id"].ToString();
                        }
                        else
                        {
                            realId = "RoomId|" + dummyId["RoomId"].ToString();
                           
                        }
                        documents.TryAdd(realId, value);
                    }

                }

                this.SetList(documents);

                this.memoryCache.Set(typeof(TEntity).Name.ToString() + "_KEY", allData);
                var dataToReturn = this.memoryCache.Get<IEnumerable<TEntity>>(typeof(TEntity).Name.ToString() + "_KEY");
                var encounterIdData = this.redisCache.StringGet(typeof(TEntity).Name.ToString() + "_KEY");
                List<string> dataIds = encounterIdData.IsNullOrEmpty ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(encounterIdData);
                foreach(var x in dataIds)
                {
                    var data = this.redisCache.StringGet(x);
                    var DeserializedData = data.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<TEntity>(data);
                }

            }
        }

        //public async Task LoadCache<TEntity>() where TEntity : class
        //{
        //    CosmosDataRetreiver cosmosDataRetriever = new CosmosDataRetreiver(this.serviceProvider);
        //    List<TEntity> allData = await cosmosDataRetriever.GetAllAsync<TEntity>();
        //    var allDataFiltered = allData.Where(x => x != null);
        //    if (allDataFiltered != null)
        //    {
        //        var cache = this.redisCache.StringSet(typeof(TEntity).Name.ToString() + "_KEY", JsonConvert.SerializeObject(allDataFiltered));
        //        //this.distributedCache.SetString("RoomDetailsKey", JsonConvert.SerializeObject(allRoomDetailsIds));

        //        var redisDataString = this.redisCache.StringGet(typeof(TEntity).Name.ToString() + "_KEY");
        //        var redisDataObject = JsonConvert.DeserializeObject<List<TEntity>>(redisDataString!);
        //        var documents = new Dictionary<RedisKey, RedisValue>();
                
        //        foreach (var item in allDataFiltered)
        //        {

        //            var value = JsonConvert.SerializeObject(item);
        //            if (item != null)
        //            {
        //                var dummyId = JObject.Parse(JsonConvert.SerializeObject(item));
        //                var realId = "";
        //                if(typeof(TEntity).Name == "RoomDetails")
        //                {
        //                    realId = dummyId["Id"].ToString();
        //                }
        //                else
        //                {
        //                    realId = dummyId["RoomId"].ToString();

        //                }
        //                documents.TryAdd(realId, value);
        //            }
                   
        //        }

        //        this.redisCache.StringSet(documents.ToArray());

        //        var encounterIdData = this.redisCache.StringGet(typeof(TEntity).Name.ToString() + "_KEY");
        //        List<TEntity> dataIds = encounterIdData.IsNullOrEmpty ? new List<TEntity>() : JsonConvert.DeserializeObject<List<TEntity>>(encounterIdData);

        //    }
        //}
    }
}
