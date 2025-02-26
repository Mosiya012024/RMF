using Azure_Room_Mate_Finder.Model;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Xml;
using User = Azure_Room_Mate_Finder.Model.User;


namespace Azure_Room_Mate_Finder.Repository
{
    public class RMFDBContext: DbContext
    {
        private readonly IConfiguration configuration;
        public RMFDBContext(DbContextOptions<RMFDBContext> options,IConfiguration configuration):base(options) {
            this.configuration = configuration;
        }

        public virtual DbSet<RoomDetails> RoomDetails {  get; set; }

        public virtual DbSet<RoomDescription> RoomDescription { get; set; }

        public virtual DbSet<User> User { get; set; }

        public virtual DbSet<LoginDto> LoginDto { get; set; }

        public virtual DbSet<LeaseContainer> LeaseContainer { get; set; }

        public virtual DbSet<ChatMessage> ChatMessage { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<RoomDetails>().ToContainer("RoomDetails").HasPartitionKey(x => x.Id);

            modelBuilder.Entity<RoomDescription>().ToContainer("RoomDescription").HasPartitionKey(x => x.RoomId).HasKey(e => e.RoomId); ;

            modelBuilder.Entity<User>().ToContainer("User").HasPartitionKey(x => x.Email).HasKey(e => e.Email);

            modelBuilder.Entity<LoginDto>().ToContainer("LoginDto").HasPartitionKey(x => x.Email).HasKey(e => e.Email);

            modelBuilder.Entity<LeaseContainer>().ToContainer("LeaseContainer").HasPartitionKey(x => x.id).HasKey(e => e.id);

            modelBuilder.Entity<ChatMessage>().ToContainer("ChatMessage").HasPartitionKey(x => x.Id).HasKey(e => e.Id);

            //modelBuilder.Entity<ChatMessage>().ToContainer("ChatMessage").HasPartitionKey(x => x.Id).Property(e=>e.ActualMessage).HasConversion<string>();







        }
    }
}
