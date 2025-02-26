using System.Linq.Expressions;

namespace Azure_Room_Mate_Finder.Repository
{
    public interface ICacheRepository<TEntity>
    {
        Task<List<TEntity>> GetAll(string key);
        
        Task<bool> UpdateMemoryCache(string key, TEntity entity,Expression<System.Func<TEntity,bool>> filter);

        Task<bool> DeleteMemoryCache(string key,TEntity entity);
    }
}