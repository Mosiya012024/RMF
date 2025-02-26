using Azure_Room_Mate_Finder.Model;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace Azure_Room_Mate_Finder.Configuration
{
    public interface IRedisCacheManager
    {
        Task UpdateAllAsync<TEntity>(IReadOnlyCollection<TEntity> collection) where TEntity: class;
    }
}
