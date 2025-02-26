using Azure_Room_Mate_Finder.Model;
using Azure_Room_Mate_Finder.Repository;
using Azure_Room_Mate_Finder.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System.Collections.ObjectModel;


namespace Azure_Room_Mate_Finder.Configuration
{
    public class RedisCacheManager : IRedisCacheManager
    {
        //private readonly IDistributedCache distributedCache;
        private readonly IServiceProvider serviceProvider;
        private readonly IDatabase redisCache;
        private readonly IConnectionMultiplexer _redis;
        private readonly IMemoryCache memoryCache;



        public RedisCacheManager(IServiceProvider serviceProvider, IConnectionMultiplexer redis, IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            this.serviceProvider = serviceProvider;
            //this.distributedCache = distributedCache;
            _redis = redis;


            redisCache = _redis.GetDatabase();

        }


        public async Task UpdateAllAsync<TEntity>(IReadOnlyCollection<TEntity> input) where TEntity : class 
        {

            if (input == null || input.Count == 0)
            {
                return;
            }

            
            List<string> updatedIds = new();
            List<string> addedIds = new();
            List<string> deletedIds = new();
            var ContainerDetails = this.redisCache.StringGet(typeof(TEntity).Name.ToString() + "_KEY");
            List<string> DeserializedContainerDetails = ContainerDetails.IsNullOrEmpty ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(ContainerDetails);


            foreach (var item in input)
            {
                var dummyId = JObject.Parse(JsonConvert.SerializeObject(item));
                var realId = dummyId["Id"].ToString().Split("|")[1];
                
                if (string.IsNullOrEmpty(realId))
                {
                    return;
                }
                if (typeof(TEntity).Name == "RoomDetails")
                {
                    
                    dummyId["Id"] = realId;
                    
                }
                else
                {
                    realId = "RoomId|" + realId;
                    
                    
                }
                var newItem = JsonConvert.SerializeObject(dummyId);
                var DesrializedNewItem = JsonConvert.DeserializeObject<TEntity>(newItem);


                var cachedRoomDetails = this.redisCache.StringGet(typeof(TEntity).Name.ToString() + "_KEY");
                List<string> DeserializedCachedRoomIds = cachedRoomDetails.IsNullOrEmpty ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(cachedRoomDetails);

                string new_updated_delete_room_detail = DeserializedCachedRoomIds.FirstOrDefault(x => x == realId);

                TEntity EntityDetail = null;
                if (new_updated_delete_room_detail != null)
                {
                    var cachedEntities = this.redisCache.StringGet(new_updated_delete_room_detail);

                    EntityDetail = JsonConvert.DeserializeObject<TEntity>(cachedEntities);
                }
               
                if (new_updated_delete_room_detail != null && JsonConvert.SerializeObject(DesrializedNewItem) != JsonConvert.SerializeObject(EntityDetail))
                {
                    updatedIds.Add(realId);

                    this.redisCache.StringSet(realId, JsonConvert.SerializeObject(DesrializedNewItem));
                    
                }
                else if (new_updated_delete_room_detail == null && !DeserializedCachedRoomIds.Contains(realId))
                {
                    DeserializedContainerDetails.Add(realId);
                    addedIds.Add(realId);
                    this.redisCache.StringSet(realId, JsonConvert.SerializeObject(DesrializedNewItem));

                }
                else
                {
                    DeserializedCachedRoomIds.Remove(realId);
                    DeserializedContainerDetails.Remove(realId);
                    deletedIds.Add(realId);
                    this.redisCache.KeyDelete(realId);
                }

            }

            this.redisCache.StringSet(typeof(TEntity).Name.ToString() + "_KEY", JsonConvert.SerializeObject(DeserializedContainerDetails));
            var redisDataString = this.redisCache.StringGet(typeof(TEntity).Name.ToString() + "_KEY");
            var redisDataObject = JsonConvert.DeserializeObject<List<string>>(redisDataString!);
            List<TEntity> EntityDetails = new List<TEntity>();
            foreach (var x in redisDataObject)
            {
                var data = this.redisCache.StringGet(x);
                var DeserializedData = data.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<TEntity>(data);
                EntityDetails.Add(DeserializedData);
            }
            this.memoryCache.Set(typeof(TEntity).Name + "_KEY", EntityDetails);
            var dataToReturn = this.memoryCache.Get<IEnumerable<TEntity>>(typeof(TEntity).Name + "_KEY");
        }

        //public async void UpdateAllAsync<TEntity>(IReadOnlyCollection<TEntity> input) where TEntity : class
        //public async void UpdateAllAsync(IReadOnlyCollection<RoomDetails> input)
        //{

        //    if (input == null || input.Count == 0)
        //    {
        //        return;
        //    }

        //    List<string> updatedIds = new();
        //    List<string> addedIds = new();
        //    List<string> deletedIds = new();

        //    foreach (var item in input)
        //    {

        //        var dummyId = JObject.Parse(JsonConvert.SerializeObject(item));
        //        var realId = dummyId["Id"].ToString().Split("|")[1];

        //        if (string.IsNullOrEmpty(realId))
        //        {
        //            return;
        //        }

        //        var cachedRoomDetails = this.redisCache.StringGet(typeof(RoomDetails).Name.ToString() + "_KEY");
        //        List<RoomDetails> DeserializedCachedRoomDetails = cachedRoomDetails.IsNullOrEmpty ? new List<RoomDetails>() : JsonConvert.DeserializeObject<List<RoomDetails>>(cachedRoomDetails);

        //        RoomDetails new_updated_delete_room_detail = DeserializedCachedRoomDetails.First(x=>x.Id == realId);


        //        if (new_updated_delete_room_detail != null && DeserializedCachedRoomDetails.Contains(item) && JsonConvert.SerializeObject(item) != JsonConvert.SerializeObject(new_updated_delete_room_detail))
        //        {
        //            updatedIds.Add(realId);

        //            this.redisCache.StringSet(realId, JsonConvert.SerializeObject(new_updated_delete_room_detail));
        //        }
        //        else if(new_updated_delete_room_detail != null && !DeserializedCachedRoomDetails.Contains(item) )
        //        {
        //            addedIds.Add(realId);
        //            this.redisCache.StringSet(realId, JsonConvert.SerializeObject(new_updated_delete_room_detail));
        //        }
        //        else
        //        {
        //            DeserializedCachedRoomDetails.Remove(item);
        //            deletedIds.Add(realId);
        //            this.redisCache.KeyDelete(realId);
        //        }

        //        this.redisCache.StringSet(typeof(RoomDetails).Name.ToString() + "_KEY", JsonConvert.SerializeObject(DeserializedCachedRoomDetails));
        //        var redisDataString = this.redisCache.StringGet(typeof(RoomDetails).Name.ToString() + "_KEY");
        //        var redisDataObject = JsonConvert.DeserializeObject<List<RoomDetails>>(redisDataString!);


        //    }
        //}
    }
}
