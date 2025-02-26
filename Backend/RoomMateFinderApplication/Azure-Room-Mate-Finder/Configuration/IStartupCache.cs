namespace Azure_Room_Mate_Finder.Configuration
{
    public interface IStartupCache
    {
        public Task InitiateRedisCache();

        public Task FlushCache();
    }
}
