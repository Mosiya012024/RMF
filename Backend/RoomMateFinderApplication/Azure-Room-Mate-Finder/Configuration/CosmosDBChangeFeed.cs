using Azure_Room_Mate_Finder.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace Azure_Room_Mate_Finder.Configuration
{
    public class CosmosDBChangeFeed : IHostedService
    {
        private readonly IHubContext<NotificationHubProxy> _hubContext;
        private readonly CosmosClient _cosmosClient;
        private readonly string _databaseName = "rmf-db"; 
        private readonly string _containerName = "RoomDetails";
        private readonly string RDContainerName = "RoomDescription";
        private readonly string _leaseContainerName = "LeaseContainer";
        private readonly IRedisCacheManager redisCacheManager;

        public CosmosDBChangeFeed(IHubContext<NotificationHubProxy> hubContext, CosmosClient cosmosClient,IRedisCacheManager redisCacheManager)
        {
            _hubContext = hubContext;
            _cosmosClient = cosmosClient;
            this.redisCacheManager = redisCacheManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Container leaseContainer = _cosmosClient.GetContainer(_databaseName, _leaseContainerName);
            Container monitoredContainer = _cosmosClient.GetContainer(_databaseName, _containerName);
            Container RDMonitoredContainer = _cosmosClient.GetContainer(_databaseName, RDContainerName);

            ChangeFeedProcessor changeFeedProcessor = monitoredContainer
                .GetChangeFeedProcessorBuilder<RoomDetails>("ChangeFeedProcessor", HandleChangesAsync)
                .WithInstanceName("YourInstanceName")
                .WithLeaseContainer(leaseContainer)
                .WithStartTime(DateTime.UtcNow)
                .Build();

            changeFeedProcessor.StartAsync();

            ChangeFeedProcessor RDchangeFeedProcessor = RDMonitoredContainer
                .GetChangeFeedProcessorBuilder<RoomDescription>("ChangeFeedProcessor", RDHandleChangesAsync)
                .WithInstanceName("YourInstanceName")
                .WithLeaseContainer(leaseContainer)
                .WithStartTime(DateTime.UtcNow)
                .Build();
            return RDchangeFeedProcessor.StartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //return Task.CompletedTask;
            return Task.CompletedTask;
        }

        private async Task HandleChangesAsync(IReadOnlyCollection<RoomDetails> changes, CancellationToken cancellationToken)
        {
            try
            {
                await this.redisCacheManager.UpdateAllAsync<RoomDetails>(changes);
                await _hubContext.Clients.All.SendAsync("ChangedRoomDetails", "Room Details Data updated", cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            
            //foreach (var change in changes)
            //{
            //    Console.WriteLine($"Document changed: {change.Id}");
            //    // Notify SignalR clients
            //    await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Data updated", cancellationToken);
            //}
        }

        private async Task RDHandleChangesAsync(IReadOnlyCollection<RoomDescription> changes, CancellationToken cancellationToken)
        {
            try
            {
                await this.redisCacheManager.UpdateAllAsync<RoomDescription>(changes);
                await _hubContext.Clients.All.SendAsync("ChangedRoomDescription", "Room Description Data updated", cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}", ex);
            }
            
        }
    }
}
