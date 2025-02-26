namespace Azure_Room_Mate_Finder.Configuration
{
    public class BackgroundChangeFeed : BackgroundService
    {
        private readonly IStartupCache startupCache;
        public BackgroundChangeFeed(IStartupCache startupCache) 
        {
            this.startupCache = startupCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Console.WriteLine("Intiating the cache");
                await this.SetCache().ConfigureAwait(false);
                Console.WriteLine("Cache is initiated");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
                
                //await Task.Delay(999999999, stoppingToken);
 
            
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // Log shutdown event or perform cleanup tasks here
            Console.WriteLine("Stopping background service...");

            // Perform any needed cleanup
            await this.FlushCache();

            Console.WriteLine("Background service stopped.");
        }


        private async Task FlushCache()
        {
            await this.startupCache.FlushCache();
        }
        private async Task SetCache()
        {
            await this.startupCache.InitiateRedisCache().ConfigureAwait(false);
        }

    }
}
