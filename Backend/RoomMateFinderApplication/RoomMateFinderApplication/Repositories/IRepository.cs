using MongoDB.Driver;

namespace RoomMateFinderApplication.Repositories
{
    public interface IRepository<TEntity>
    {
        public Task<List<TEntity>> FindAllAsync();  
        
        public void CreateAsync(TEntity entity);

        public Task<List<TEntity>> CreateMultipleAsync(IList<TEntity> entities);

        public Task<List<TEntity>> DeleteMultipleAsync(int number);

        public Task<IMongoCollection<TEntity>> getCollection();
    }
}
