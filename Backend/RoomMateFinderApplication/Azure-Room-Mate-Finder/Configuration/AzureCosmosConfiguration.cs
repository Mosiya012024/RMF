using Azure_Room_Mate_Finder.Model;
using Azure_Room_Mate_Finder.Repository;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.EntityFrameworkCore;
using System;

namespace Azure_Room_Mate_Finder.Configuration
{
    public static class AzureCosmosConfiguration
    {

       
        //private readonly IConfiguration configuration;
        //public static AzureCosmosConfiguration(IConfiguration configuration) {
        //    this.configuration = configuration;
        //}

        //public IServiceCollection AddCosmos(IServiceCollection services)
        //{
        //    var AccountURI = this.configuration.GetValue<string>("CosmosDB:AccountURI");
        //    var PrimaryKey = this.configuration.GetValue<string>("CosmosDB:PrimaryKey");
        //    var AccountName = this.configuration.GetValue<string>("CosmosDS:AccountName");
        //    var DatabaseName = this.configuration.GetValue<string>("CosmosDS:DatabaseName");
        //    var ContainerName = this.configuration.GetValue<string>("CosmosDS:ContainerName");
        //    //this is used to connect Azure Cosmos DB account with .NET Core Application
        //    services.AddSingleton(_ => new CosmosClientBuilder($"AccountEndpoint={AccountURI};AccountKey={PrimaryKey};").Build());

        //    //to connect with the azure cosmos database and its containers use this.
        //    return services;


        //}


        public static IServiceCollection AddCosmos(this IServiceCollection services, IConfiguration configuration)
        {

            //this is used to connect Azure Cosmos DB account with .NET Core Application
            //services.AddSingleton(_ => new CosmosClientBuilder($"AccountEndpoint={AccountURI};AccountKey={PrimaryKey};").Build());

            

            //to connect the total azure cosmos database with the ASP.NET Core and its containers use this dbContext and dbSet.
            services.AddDbContext<RMFDBContext>(options => {
                var AccountURI = configuration["CosmosDB:AccountURI"];
                var PrimaryKey = configuration["CosmosDB:PrimaryKey"];
                var AccountName = configuration["CosmosDB:AccountName"];
                var DatabaseName = configuration["CosmosDB:DatabaseName"];
                var ContainerName = configuration["CosmosDB:ContainerName"];
                options.UseCosmos(accountEndpoint: AccountURI, accountKey: PrimaryKey, databaseName: DatabaseName);
                
                });

            
            return services;
        }

        //public static async Task InitializeDb(RMFDBContext context)
        //{
        //    if (context.RoomDetails.ToListAsync().Result.Count == 0)
        //    {
        //        context.RoomDetails.Add(new RoomDetails { Id = Guid.NewGuid().ToString(),
        //            Name = "Emery",
        //            Address =  "FJDGJKRDG",
        //            Requirement = "uyeiyiwt",
        //            Amount = 10200,
        //            Status =  "Vacant",
        //            Identity =  "Room Finder",
        //        });
        //        await context.SaveChangesAsync();
        //    }
        //}
    }
}
