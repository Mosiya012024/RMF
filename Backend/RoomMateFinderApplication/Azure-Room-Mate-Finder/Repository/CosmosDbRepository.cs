using Azure_Room_Mate_Finder.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Xml;

namespace Azure_Room_Mate_Finder.Repository
{
    public class CosmosDbRepository<TEntity>:ICosmosDbRepository<TEntity> where TEntity : class
    {
        private readonly RMFDBContext context;

        public CosmosDbRepository(RMFDBContext context)
        {
            this.context = context;
        }

        public async Task<List<TEntity>> FindAllAsync(Expression<System.Func<TEntity, bool>> filter)
        {
            //just to check that the connection between Cosmos DB and .NET Core application is set or not.
            //var cosmosClient = new CosmosClient("https://room-mate-finder-manager.documents.azure.com:443/", "");
            //var container = cosmosClient.GetContainer("rmf-db", "RoomDetails");

            //var iterator = container.GetItemQueryIterator<RoomDetails>("SELECT * FROM c");
            //var results = new List<RoomDetails>();

            //while (iterator.HasMoreResults)
            //{
            //    var response = await iterator.ReadNextAsync();
            //    results.AddRange(response);
            //}


            //var AllRooms = await this.context.RoomDetails.ToListAsync();
            //var TotallRooms = this.context.RoomDetails.ToList();

            ////return AllRooms;
            //DbSet<RoomDetails> dbSet = this.context.Set<RoomDetails>();
            //var abcde = await dbSet.ToListAsync().ConfigureAwait(false);
            //return abcde;

            Collection<TEntity> collections = new Collection<TEntity>();
            DbSet<TEntity> dbSet = this.context.Set<TEntity>();
            return await (filter == null ? dbSet.ToListAsync().ConfigureAwait(false) : dbSet.Where(filter).ToListAsync().ConfigureAwait(false));

        }

        public async Task<List<TEntity>> PostAsync(TEntity entity)
        {
            DbSet<TEntity> dbSet = this.context.Set<TEntity>();
            await dbSet.AddAsync(entity).ConfigureAwait(false);
            await this.context.SaveChangesAsync().ConfigureAwait(false);
            return await dbSet.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<TEntity>> PostAllAsync(IList<TEntity> entities)
        {
            DbSet<TEntity> dbSet = this.context.Set<TEntity>();
            await dbSet.AddRangeAsync(entities).ConfigureAwait(false);
            await this.context.SaveChangesAsync().ConfigureAwait(false);
            return await dbSet.ToListAsync().ConfigureAwait(false);

        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {

            DbSet<TEntity> dbSet = this.context.Set<TEntity>();
            
            await dbSet.AddAsync(entity).ConfigureAwait(false);
            this.context.Entry<TEntity>(entity).State = EntityState.Modified;
            await this.context.SaveChangesAsync().ConfigureAwait(false);
            return entity;
        }

        public async Task<List<TEntity>> DeleteAsync(TEntity entity)
        {
            DbSet<TEntity> dbSet = this.context.Set<TEntity>();
            dbSet.Remove(entity);
            await this.context.SaveChangesAsync().ConfigureAwait (false);
            return await dbSet.ToListAsync().ConfigureAwait(false);
        }

    }
}
