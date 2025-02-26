using Microsoft.EntityFrameworkCore;

namespace Azure_Room_Mate_Finder.Repository
{
    public interface IRMFDBContext
    {
        DbSet<T> Set<T>() where T : class;

    }

}
