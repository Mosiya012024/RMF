using Azure_Room_Mate_Finder.Configuration;
using Azure_Room_Mate_Finder.Repository;
using Azure_Room_Mate_Finder.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;
using Serilog.Events;
using Serilog;
using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Text;
using StackExchange.Redis;
using Microsoft.OpenApi.Models;
using Azure_Room_Mate_Finder.Model;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using static System.Net.WebRequestMethods;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// this code was commented while publishing to Azure webb app service
var cosmosClient = new CosmosClient(builder.Configuration.GetValue<string>("CosmosDB:AccountURI"),
                                    builder.Configuration.GetValue<string>("CosmosDB:PrimaryKey"));
var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(builder.Configuration.GetValue<string>("CosmosDB:DatabaseName"));

Container roomDetailsContainer = await database.Database.CreateContainerIfNotExistsAsync("RoomDetails", "/Id");
Container roomDescriptionContainer = await database.Database.CreateContainerIfNotExistsAsync("RoomDescription", "/RoomId");
Container userContainer = await database.Database.CreateContainerIfNotExistsAsync("User", "/Email");
Container loginDtoContainer = await database.Database.CreateContainerIfNotExistsAsync("LoginDto", "/Email");
Container LeaseContainer = await database.Database.CreateContainerIfNotExistsAsync("LeaseContainer", "/id");
Container chatMessageContainer = await database.Database.CreateContainerIfNotExistsAsync("ChatMessage", "/Id");

//Register these containers as singleton services
builder.Services.AddSingleton(roomDetailsContainer);
builder.Services.AddSingleton(roomDescriptionContainer);
builder.Services.AddSingleton(userContainer);
builder.Services.AddSingleton(loginDtoContainer);
builder.Services.AddSingleton(LeaseContainer);
builder.Services.AddSingleton(chatMessageContainer);

//builder.Services.AddSingleton(async (_) =>
//{
//    CosmosClient _cosmosClient = new CosmosClient(builder.Configuration.GetValue<string>("CosmosDB:AccountURI"), builder.Configuration.GetValue<string>("CosmosDB:PrimaryKey"));
//    Database _database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(builder.Configuration.GetValue<string>("CosmosDB:DatabaseName"));
//    Container roomDetailsContainer = await _database.CreateContainerIfNotExistsAsync("RoomDetails", "Identity");
//    Container roomDescriptionContainer = await _database.CreateContainerIfNotExistsAsync("RoomDescription", "Gender");
//    Container UserContainer = await _database.CreateContainerIfNotExistsAsync("User", "Email");
//    Container LoginDtoContainer = await _database.CreateContainerIfNotExistsAsync("LoginDto", "Email");
//});


//builder.Services.AddSingleton(provider =>
//{

//    var AccountURI = builder.Configuration.GetValue<string>("CosmosDB:AccountURI");
//    return new CosmosClient(AccountURI, builder.Configuration.GetValue<string>("CosmosDB:PrimaryKey"));
//});

//builder.Services.AddSingleton(async provider =>
//{
//    var cosmosClient = provider.GetRequiredService<CosmosClient>();
//    Database _database = await cosmosClient.CreateDatabaseIfNotExistsAsync(builder.Configuration.GetValue<string>("CosmosDB:DatabaseName"));
//    Container roomDetailsContainer = await _database.CreateContainerIfNotExistsAsync("RoomDetails", "Identity");
//    Container roomDescriptionContainer = await _database.CreateContainerIfNotExistsAsync("RoomDescription", "Gender");
//    Container userContainer = await _database.CreateContainerIfNotExistsAsync("User", "Email");
//    Container loginDtoContainer = await _database.CreateContainerIfNotExistsAsync("LoginDto", "Email");

//    return new
//    {
//        RoomDetailsContainer = roomDetailsContainer,
//        RoomDescriptionContainer = roomDescriptionContainer,
//        UserContainer = userContainer,
//        LoginDtoContainer = loginDtoContainer
//    };
//});


builder.Services.AddCosmos(builder.Configuration);

builder.Services.AddTransient(typeof(ICosmosDbRepository<>), typeof(CosmosDbRepository<>));
builder.Services.AddTransient<IRoomMateFinderManager, RoomMateFinderManager>();
builder.Services.AddTransient<IUserManager, UserManager>();
builder.Services.AddSingleton(new JwtService("nS5R&y9Xj!L7kZ2w^q3Pv8H+fQxD4mB"));
builder.Services.AddHttpClient();
var key = Encoding.ASCII.GetBytes("nS5R&y9Xj!L7kZ2w^q3Pv8H+fQxD4mB"); // Replace with your actual key
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };

    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            // If the request is for SignalR
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

//builder.Services.AddSingleton(serviceProvider =>
//{
//    return new CosmosClient("https://room-mate-finder.documents.azure.com:443/", "");
//});




//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll",
//        builder =>
//        {
//            builder.WithOrigins("http://localhost:3000")
//                   .AllowAnyHeader()
//                   .AllowAnyMethod()
//                   .AllowCredentials();
//        });
//});



//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(builder =>
//    {
//        builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
//    });
//});

builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
{
    builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithOrigins("http://localhost:3000");
}));

//builder.Services.AddApplicationInsightsTelemetry();
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["ApplicationInsights:InstrumentationKey"]);
builder.Logging.ClearProviders(); //Optional: Clears other logging providers if you only want console logging
builder.Logging.AddConsole();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console() // Optional: Log to the console
    .WriteTo.ApplicationInsights(new TelemetryConfiguration
    {
        InstrumentationKey = builder.Configuration["ApplicationInsights:InstrumentationKey"]
    }, TelemetryConverter.Traces) // Log to Application Insights
    .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddSingleton((s) =>
{
    // Ensure the connection string is set in your configuration (appsettings.json or environment variables)
    string connectionString = builder.Configuration["CosmosDB:ConnectionString"];
    return new CosmosClient(connectionString);
});
//Adding SignalR service
builder.Services.AddSignalR();

builder.Services.AddHostedService<CosmosDBChangeFeed>();

//builder.Services.AddSingleton<IDictionary<string, UserConnection>>(IServiceProvider =>
//    new Dictionary<string, UserConnection>());

//builder.Services.AddSingleton<IDictionary<string, string>>(IServiceProvider =>
//    new Dictionary<string, string>());

builder.Services.AddSingleton<user_conn_ids_dict>(sp =>
    new user_conn_ids_dict
    {
    });
builder.Services.AddSingleton<chat_room_user_name_dict>(sp =>
    new chat_room_user_name_dict
    {
    });
builder.Services.AddSingleton<username_chatroom_dict>(IServiceProvider =>
    new username_chatroom_dict { });
builder.Services.AddSingleton<users_dict>(IServiceProvider =>
    new users_dict { });

//adding redis cache connection
//builder.Services.AddDistributedRedisCache((options) =>
//{
//    options.Configuration = builder.Configuration.GetValue<string>("RedisCache:ConnectionString");
//});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetValue<string>("RedisCache:ConnectionString");
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddTransient<IRedisCacheManager, RedisCacheManager>();

builder.Services.AddSingleton<IStartupCache, StartupCache>();

builder.Services.AddTransient(typeof(ICacheRepository<>), typeof(CacheRepository<>));

builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<BackgroundChangeFeed>();

builder.Services.AddMemoryCache();



////adding Azure KeyVault to .net application
//var keyVaultName = builder.Configuration.GetValue<string>("KeyVault:Name");
////var keyVaultURI = new Uri($"https://{keyVaultName}.vault.azure.net/");
//var keyVaultURI = "https://rmf-app-key-vault.vault.azure.net/";
//var Client = new SecretClient(new Uri(keyVaultURI), new DefaultAzureCredential());
//KeyVaultSecret keyVaultSecret = Client.GetSecret("RedisCacheConnectionString");


//this code is working just commented out as I didn't create azure key vault
//var keyVaultUri = builder.Configuration.GetValue<string>("KeyVault:ConnectionURI");

//if (!string.IsNullOrEmpty(keyVaultUri))
//{
//    // Add Azure Key Vault to the app's configuration
//    builder.Configuration.AddAzureKeyVault(
//        new Uri(keyVaultUri),
//        new DefaultAzureCredential());
//}

//Role Based Access Control and Policy Based Access Control
//builder.Services.AddAuthorization(
//    options =>
//    {
//        options.AddPolicy("UserType", policy => policy.RequireClaim(ClaimTypes.Role, "Room Mate Finder")
//        );
//    }
//    );

builder.Services.AddAuthorization(
    options =>
    {
        options.AddPolicy("UserType", policy => policy.RequireAssertion(abc => abc.User.HasClaim(c=>c.Type == ClaimTypes.Role && c.Value == "Room Mate Finder")
        || abc.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Room Finder")
        ));
    }
    );

//injecting Email Service and configuration to send email to user's mail ID
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<EmailService>();

//for using azure app configuration in application
builder.Configuration.AddAzureAppConfiguration(builder.Configuration.GetValue<string>("AppConfiguration:ConnectionString"));

var app = builder.Build();


//Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}


app.UseHttpsRedirection();

//app.UseCors();

app.UseCors("CorsPolicy");


app.UseAuthentication();


app.MapControllers();


app.UseRouting();

app.UseAuthorization();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapHub<NotificationHub>("/notificationHub");
//});

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHubProxy>("/notificationHub");
});
app.Run();
