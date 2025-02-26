using Azure_Room_Mate_Finder.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Azure_Room_Mate_Finder.Repository
{
    public interface ICosmosDbRepository<TEntity>
    {
        public Task<List<TEntity>> FindAllAsync(Expression<System.Func<TEntity, bool>> filter);

        public Task<List<TEntity>> PostAsync(TEntity entity);

        public Task<List<TEntity>> PostAllAsync(IList<TEntity> entities);

        public Task<TEntity> UpdateAsync(TEntity entity);

        public Task<List<TEntity>> DeleteAsync(TEntity entity);

        
    }
}
