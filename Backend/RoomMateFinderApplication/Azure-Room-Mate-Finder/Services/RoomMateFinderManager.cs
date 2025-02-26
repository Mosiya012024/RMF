
using Azure_Room_Mate_Finder.Model;
using Azure_Room_Mate_Finder.Repository;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using Expression = System.Linq.Expressions.Expression;
using System.Diagnostics;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO.Compression;
using System.Net.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Runtime.CompilerServices;
using Azure_Room_Mate_Finder.Configuration;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Azure_Room_Mate_Finder.Controllers;


namespace Azure_Room_Mate_Finder.Services
{
    [ExcludeFromCodeCoverage]
    public class RoomMateFinderManager : IRoomMateFinderManager
    {
        private readonly ICosmosDbRepository<RoomDetails> roomDetailsRepo;
        private readonly ICosmosDbRepository<RoomDescription> roomDescriptionRepo;
        private readonly ICacheRepository<RoomDetails> roomDetailsCacheRepo;
        private readonly ICacheRepository<RoomDescription> roomDescriptionCacheRepo;
        private readonly List<Address> possibleAddresses;
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;
        private readonly IRedisCacheManager redisCacheManager;
        private readonly IHubContext<NotificationHubProxy> hubContext;


        public RoomMateFinderManager(ICosmosDbRepository<RoomDetails> roomDetailsRepo,
            ICosmosDbRepository<RoomDescription> roomDescriptionRepo,
            IConfiguration configuration, HttpClient httpClient,
            IRedisCacheManager redisCacheManager,
            ICacheRepository<RoomDetails> roomDetailsCacheRepo,
            ICacheRepository<RoomDescription> roomDescriptionCacheRepo,
            IHubContext<NotificationHubProxy> hubContext
            )
        {
            this.roomDetailsRepo = roomDetailsRepo;
            AddressDuplicateData AddressGenerator = new AddressDuplicateData();
            possibleAddresses = AddressGenerator.getDuplicateAddresses();
            this.roomDescriptionRepo = roomDescriptionRepo;
            this.configuration = configuration;
            this.httpClient = httpClient;
            this.redisCacheManager = redisCacheManager;
            this.roomDetailsCacheRepo = roomDetailsCacheRepo;
            this.roomDescriptionCacheRepo = roomDescriptionCacheRepo;
            this.hubContext = hubContext;
        }

        public List<RoomDetails> GetRoomsOrderBy(string orderBy, List<RoomDetails> allRooms)
        {
            
            return orderBy?.ToUpperInvariant() switch
            {
                "NAME_ASC" => allRooms.OrderBy(s => s.Name).ToList(),
                "NAME_DESC" => allRooms.OrderByDescending(s => s.Name).ToList(),
                "ADDRESS_ASC" => allRooms.OrderBy(s => s.Name).ToList(),
                "ADDRESS_DESC" => allRooms.OrderByDescending(s => s.Name).ToList(),
                "AMOUNT_ASC" => allRooms.OrderBy(s => s.Amount).ToList(),
                "AMOUNT_DESC" => allRooms.OrderByDescending(s => s.Amount).ToList(),
                "GENDER_ASC" => allRooms.OrderBy(s => s.Requirement.Gender).ToList(),
                "GENDER_DESC" => allRooms.OrderByDescending(s => s.Requirement.Gender).ToList(),
                "VACANCY_ASC" => allRooms.OrderBy(s => s.Requirement.Vacancy).ToList(),
                "VACANCY_DESC" => allRooms.OrderByDescending(s => s.Requirement.Vacancy).ToList(),
                "SIZE_ASC" => allRooms.OrderBy(s => s.Requirement.Size).ToList(),
                "SIZE_DESC" => allRooms.OrderByDescending(s => s.Requirement.Size).ToList(),
                "STATUS_ASC" => allRooms.OrderBy(s => s.Status).ToList(),
                "STATUS_DESC" => allRooms.OrderByDescending(s => s.Status).ToList(),
                "IDENTITY_ASC" => allRooms.OrderBy(s => s.Identity).ToList(),
                "IDENTITY_DESC" => allRooms.OrderByDescending(s => s.Identity).ToList(),
                _ => allRooms.ToList()
            };

        }
        public async Task<List<RoomDetails>> GetAllRooms(string filter, string orderBy)
        {   
            //actual code 
            //var rooms = await this.roomDetailsRepo.FindAllAsync();


            //will remove this code afterwards
            //var allRooms =  await mongoDbService.GetUsersAsync(filterPredicate);
            //return allRooms;
            var filterPredicate = this.FindFilterExpression(filter);
            //IMongoCollection<RoomDetails> collection = await this.roomDetailsRepo.getCollection();
            //var allRooms = filterPredicate == null ? collection.Find(x=>true).ToList() : await collection.Find(filterPredicate).ToListAsync();
            //var allRooms = await this.roomDetailsRepo.FindAllAsync(filterPredicate);
            var allRooms = await this.GetRoomDetailsFromMemoryCache(filterPredicate);
            
            var sortedRooms = GetRoomsOrderBy(orderBy, allRooms);
            return sortedRooms;
        }

        public async Task<List<RoomDetails>> GetRoomDetailsFromMemoryCache(Expression<Func<RoomDetails, bool>> filter)
        {
            var allEntities = await this.roomDetailsCacheRepo.GetAll("RoomDetails_KEY").ConfigureAwait(false);
            if (allEntities == null || allEntities.Count == 0)
            {
                allEntities = await this.roomDetailsRepo.FindAllAsync(null).ConfigureAwait(false);
                this.redisCacheManager.UpdateAllAsync<RoomDetails>(allEntities);
            }

            return filter == null ? allEntities : allEntities.AsQueryable().Where(filter).ToList();
        }

        public async Task<List<RoomDescription>> GetRoomDescriptionFromMemoryCache(Expression<Func<RoomDescription, bool>> filter)
        {
            var allEntities = await this.roomDescriptionCacheRepo.GetAll("RoomDescription_KEY").ConfigureAwait(false);
            if (allEntities == null || allEntities.Count == 0)
            {
                allEntities = await this.roomDescriptionRepo.FindAllAsync(null).ConfigureAwait(false);
                this.redisCacheManager.UpdateAllAsync<RoomDescription>(allEntities);
            }

            return filter == null ? allEntities : allEntities.AsQueryable().Where(filter).ToList();
        }

        public async Task<RoomDetails> PostRoom(RoomDetails roomDetails)
        {
            roomDetails.Id = Guid.NewGuid().ToString();
            await this.roomDetailsRepo.PostAsync(roomDetails);
            return roomDetails;
        }

        public async Task<List<RoomDescription>> PostRoomCombinedDetails(RoomCombinedDetails roomCombinedDetails)
        {
            RoomDetails roomDetails = new RoomDetails()
            {
                Id = roomCombinedDetails.Id,
                Name = roomCombinedDetails.Name,
                Address = roomCombinedDetails.Address,
                Requirement = roomCombinedDetails.Requirement,
                Amount = roomCombinedDetails.Amount,
                Status = roomCombinedDetails.Status,
                Identity = roomCombinedDetails.RoomType == "Room" ? "Room Mate Finder" : "Room Finder",
                AssignedTo = new List<string>(),
            };
            await this.roomDetailsRepo.PostAsync(roomDetails);
            RoomDescription roomDescription = new RoomDescription()
            {
                RoomId = roomDetails.Id,
                FlatNumber = roomCombinedDetails.FlatNumber,
                AmenityType = roomCombinedDetails.AmenityType,
                Amenities = roomCombinedDetails.Amenities,
                Rent = roomCombinedDetails.Amount,
                Deposit = roomCombinedDetails.Deposit,
                Maintenance = roomCombinedDetails.Maintenance,
                Gender = roomCombinedDetails.Requirement.Gender,
                PostedOn = roomCombinedDetails.PostedOn,
                AvailableFrom = roomCombinedDetails.AvailableFrom,
                Size = roomCombinedDetails.Requirement.Size,
                State = roomCombinedDetails.Address.State,
            };
             return await this.roomDescriptionRepo.PostAsync(roomDescription);

        }

        public async Task<List<RoomDetails>> PostMultipleRooms(int number)
        {

            var blobStorageConnectionString = this.configuration.GetValue<string>("StorageAccount:ConnectionString");
            var ContainerName = this.configuration.GetValue<string>("StorageAccount:ContainerName");
            BlobServiceClient blobInstance = new BlobServiceClient(blobStorageConnectionString);
            BlobContainerClient blobContainerName = blobInstance.GetBlobContainerClient(ContainerName);

            IList<RoomDetails> roomsAdded = new List<RoomDetails>();
            IList<RoomDescription> roomDescriptionsAdded = new List<RoomDescription>();
            List<string> possibleStatus = new List<string>() {"Vacant","Occupied"};
            List<string> possibleGender = new List<string>() {"Male","Female","Any"};
            List<string> possibleSize = new List<string>() {"1BHK","2BHK","3BHK","4BHK","5BHK"};
            List<string> possibleNames = new List<string>() {  "Liam", "Noah", "Oliver", "Elijah", "William", "James", "Benjamin", "Lucas", "Henry", "Alexander",
            "Mason", "Michael", "Ethan", "Daniel", "Jacob", "Logan", "Jackson", "Levi", "Sebastian", "Mateo",
            "Jack", "Owen", "Theodore", "Aiden", "Samuel", "Joseph", "John", "David", "Wyatt", "Matthew",
            "Luke", "Asher", "Carter", "Julian", "Grayson", "Leo", "Jayden", "Gabriel", "Isaac", "Lincoln",
            "Anthony", "Hudson", "Dylan", "Ezra", "Thomas", "Charles", "Christopher", "Jaxon", "Maverick", "Josiah",
            "Isaiah", "Andrew", "Elias", "Joshua", "Nathan", "Caleb", "Ryan", "Adrian", "Miles", "Eli",
            "Nolan", "Christian", "Aaron", "Cameron", "Ezekiel", "Colton", "Luca", "Landon", "Hunter", "Jonathan",
            "Santiago", "Axel", "Easton", "Cooper", "Jeremiah", "Angel", "Roman", "Connor", "Jameson", "Robert",
            "Greyson", "Jordan", "Ian", "Carson", "Jaxson", "Leonardo", "Nicholas", "Dominic", "Austin", "Everett",
            "Brooks", "Xavier", "Kai", "Jose", "Parker", "Adam", "Jace", "Wesley", "Kayden", "Silas",
             "Olivia", "Emma", "Ava", "Charlotte", "Sophia", "Amelia", "Isabella", "Mia", "Evelyn", "Harper",
            "Camila", "Gianna", "Abigail", "Luna", "Ella", "Elizabeth", "Sofia", "Emily", "Avery", "Mila",
            "Scarlett", "Eleanor", "Madison", "Layla", "Penelope", "Aria", "Chloe", "Grace", "Ellie", "Nora",
            "Hazel", "Zoey", "Riley", "Victoria", "Lily", "Aurora", "Violet", "Nova", "Hannah", "Emilia",
            "Zoe", "Stella", "Everly", "Isla", "Leah", "Lillian", "Addison", "Willow", "Lucy", "Paisley",
            "Natalie", "Naomi", "Eliana", "Brooklyn", "Elena", "Aubrey", "Claire", "Ivy", "Kinsley", "Audrey",
            "Maya", "Genesis", "Skylar", "Bella", "Aaliyah", "Madelyn", "Savannah", "Anna", "Delilah", "Serenity",
            "Caroline", "Kennedy", "Valentina", "Ruby", "Sophie", "Alice", "Gabriella", "Sadie", "Ariana", "Allison",
            "Hailey", "Autumn", "Nevaeh", "Natalia", "Quinn", "Josephine", "Sarah", "Cora", "Emery", "Samantha",
            "Piper", "Leilani", "Eva", "Everleigh", "Madeline", "Lydia", "Jade", "Peyton", "Brielle", "Adeline"
            };
            List<int> availableFromDates = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
            List<int> flatNumbers = new List<int>() { 3062, 1332, 5464, 3243, 9892, 9821, 4481, 8311, 6137, 1839, 1718,7378,4942,1981 };
            List<string> AmenitiesType = new List<string>() {"Fully Furnished","Semi Furnished","Un Furnished"};
            List<string> userIdentities = new List<string>() { "Room Finder", "Room Mate Finder" };
            //List<string> Occupancy = new List<string>() { "Single", "2 - Shared", "3 - Shared", "4 - Shared" };
            Dictionary<string, string> OccupancyDict = new Dictionary<string, string>()
            {
                { "1BHK", "Single"} , { "2BHK", "2 - Shared"}, { "3BHK", "3 - Shared"}, { "4BHK", "4 - Shared"},{"5BHK", "5 - Shared"}
            };
            
            List<string> RoomImages = new List<string>()
            {
                "https://content.jdmagicbox.com/comp/def_content/hostel-for-boy-students/550e215938-hostel-for-boy-students-3-58t3u.jpg",
                "https://content.jdmagicbox.com/comp/def_content/hostel-for-boy-students/e7551345ac-hostel-for-boy-students-5-5j8em.jpg",
                "https://content.jdmagicbox.com/comp/kozhikode/w5/0495px495.x495.170814153626.x7w5/catalogue/viswadheepam-paying-guest-kozhikode-9mper-250.jpg",
                "https://content.jdmagicbox.com/comp/kollam/f5/9999px474.x474.170922095429.b1f5/catalogue/zion-hostels-for-boys-chinnakada-kollam-hostels-1dvu4wsmpv.jpg",
                "https://content.jdmagicbox.com/comp/alappuzha/t7/0477px477.x477.180528222517.v3t7/catalogue/kovilakam-ladies-hostel-kayangulam-alappuzha-paying-guest-accommodations-f93wb.jpeg",
                "https://content.jdmagicbox.com/comp/def_content/hostel-for-students/c5a1a9ec71-hostel-for-students-4-hh8lx.jpg",
                "https://content.jdmagicbox.com/comp/srikakulam/a1/9999p8942.8942.190319215726.g1a1/catalogue/yuvathi-ladies-hostel-p-g--srikakulam-paying-guest-accommodations-zkefhpdqn7.jpg",
                "https://content.jdmagicbox.com/v2/comp/hyderabad/f5/040pxx40.xx40.220317165958.c5f5/catalogue/b-ed-hostel-ou-osmania-university-hyderabad-hostel-for-boy-students-1ny9lirqkf.jpg",
                "https://content3.jdmagicbox.com/comp/pune/z6/020pxx20.xx20.200318194500.g7z6/catalogue/tribe-stays-student-hostel-clover-park-view-pune-hostels-hfusjli1nj.jpg",
                "https://content.jdmagicbox.com/v2/comp/bangalore/d7/080pxx80.xx80.190515193339.b2d7/catalogue/tribe-student-hostel-doddaballapur-bangalore-hostels-glwc7eank5-250.jpg",
            };
            while (number > 0)
            {
                Random rand = new Random();
                var shuffledStatus = possibleStatus.OrderBy(x => rand.Next()).ToList();
                var shuffeldGender = possibleGender.OrderBy(x => rand.Next()).ToList();
                var shuffledSize = possibleSize.OrderBy(x => rand.Next()).ToList();
                var index = rand.Next(0, possibleAddresses.Count());
                Address shuffledAddress = possibleAddresses[index];

                
                RoomDetails room = new RoomDetails()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = possibleNames[rand.Next(0, possibleNames.Count)],
                    Address = new Address()
                    {
                        Area = shuffledAddress.Area,
                        City = shuffledAddress.City,
                        State = shuffledAddress.State,
                    },
                    Requirement = new Requirement(),
                    Amount = rand.Next(12, 21) * 600,
                    Status = shuffledStatus[0],
                    Identity = userIdentities[rand.Next(0, userIdentities.Count)],
                    AssignedTo = new List<string>(),
                };
                
                room.SetRequirementIdentity();
                roomsAdded.Add(room);
                RoomDescription rd = new RoomDescription()
                {
                    Id = "",
                    RoomId = room.Id,
                    FlatNumber = room.Identity.Equals("Room Mate Finder") ? ("Flat No " + flatNumbers[rand.Next(0, flatNumbers.Count)] + " " + room.Address.Area + " " + room.Address.City + " " + room.Address.State) : room.Address.Area + ", " + room.Address.City + ", " + room.Address.State,
                    AmenityType = AmenitiesType[rand.Next(0, AmenitiesType.Count)],
                    Amenities = new List<string>(),
                    Deposit = rand.Next(12, 21) * 60,
                    Maintenance = room.Identity.Equals("Room Mate Finder") ? rand.Next(12, 21) * 6 : 0,
                    PostedOn = DateTime.Now,
                    AvailableFrom = DateTime.Now.AddDays(availableFromDates[rand.Next(0, availableFromDates.Count)]),
                    State = room.Address.State,
                    Gender = room.Requirement.Gender,
                    Rent = room.Amount,
                    Size = room.Requirement.Size,
                    //Size = room.Identity.Equals("Room Mate Finder") ? room.Requirement.Size : OccupancyDict.GetValueOrDefault(room.Requirement.Size) ?? "Single",
                };
                rd.SetAmenities();
                // this.roomDescriptionRepo.CreateAsync(rd);
                roomDescriptionsAdded.Add(rd);

                for (int i = 0; i < 3; i++)
                {
                    string imageUrl = RoomImages[rand.Next(0, RoomImages.Count)];
                    BlobClient blobClient = blobContainerName.GetBlobClient(room.Id + "/" + Guid.NewGuid().ToString() + "_" + Path.GetExtension(imageUrl).ToLower());

                    // Upload the byte array as a stream to Azure Blob Storage
                    var imageResponse = await httpClient.GetAsync(imageUrl);
                    var imageStream = await imageResponse.Content.ReadAsStreamAsync();

                    var mimeType = GetMimeType(imageUrl);

                    // Upload the image to Azure Blob Storage with the correct MIME type
                    var uploadOptions = new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders
                        {
                            ContentType = mimeType
                        }
                    };

                    // Upload the image to Azure Blob Storage
                    await blobClient.UploadAsync(imageStream, uploadOptions);
                }

                number--;
            }

            //List<string> abcd = new List<string>()
            //{
            //    "Model", "Apple", "Cat","Ball"
            //};
            //abcd.Sort();
            //abcd.Add(abcd[0]);
            

            //IList implementation
            //IList<string> bcde = new List<string>();
            //bcde.Add(string.Empty);

            await this.roomDescriptionRepo.PostAllAsync(roomDescriptionsAdded);

            var allRooms = await this.roomDetailsRepo.PostAllAsync(roomsAdded);

            return allRooms;
        }

        private string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".jfif" => "image/jpeg", // Handle .jfif extension as image/jpeg
                _ => "application/octet-stream" // Default MIME type
            };
        }


        public async Task<List<RoomDetails>> DeleteMultipleRooms(int number)
        {
            var allItems = await this.roomDetailsRepo.FindAllAsync(null);
            if (allItems.Count < number)
            {
                //throw new CustomException();
                throw new Exception("Not enough Data to delete");
            }
            else
            {
                while (number > 0)
                {

                    var firstItem = allItems[0];
                    var firstRoomDescriptionItem = await this.roomDescriptionRepo.FindAllAsync(x => x.RoomId == firstItem.Id);
                    await this.roomDescriptionRepo.DeleteAsync(firstRoomDescriptionItem[0]);
                    //collection.DeleteOne(x => x.Equals(firstItem));
                    await this.roomDetailsRepo.DeleteAsync(firstItem);
                    allItems = await this.roomDetailsRepo.FindAllAsync(null);
                    number--;
                }
            }
            return allItems;
        }

        private Model.Expression FindInnerExpressions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter) || string.IsNullOrEmpty(filter))
            {
                return null;
            }
            Model.Expression filterExpression = new Model.Expression
            {
                InnerExpressions = new List<InnerExpressions>()
            };
            string[] filtersArray = filter.Split("(and)", StringSplitOptions.RemoveEmptyEntries);
            foreach (string array in filtersArray)
            {
                string[] array3 = array.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (array3.Length < 2)
                {
                    throw new Exception("filter is of incorrect format");
                }
                StringBuilder stBuilder = new StringBuilder(array3[2]);

                for (int i = 3; i < array3.Length; i++)
                {
                    stBuilder.Append(" " + array3[i]);
                }
                filterExpression.InnerExpressions.Add(new InnerExpressions()
                {
                    LHS = array3[0],
                    Operator = array3[1],
                    RHS = stBuilder.ToString().Trim(),
                });
            }
            return filterExpression;
        }


        public static Expression<Func<RoomDetails, bool>> AndAlso(Expression<Func<RoomDetails, bool>> expr1, Expression<Func<RoomDetails, bool>> expr2)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(RoomDetails));

            var body = System.Linq.Expressions.Expression.AndAlso(
                System.Linq.Expressions.Expression.Invoke(expr1, parameter),
                System.Linq.Expressions.Expression.Invoke(expr2, parameter)
            );

            return Expression.Lambda<Func<RoomDetails, bool>>(body, parameter);
        }
        private Expression<System.Func<RoomDetails, bool>> FindFilterExpression(string filter)
        {
            var allExpressions = FindInnerExpressions(filter);

            Expression<Func<RoomDetails, bool>> totalExpression = null;

            

            if (allExpressions != null) {
                foreach (var smallExceptions in allExpressions.InnerExpressions)
                {
                    if (smallExceptions.LHS == "gender")
                    {
                        Expression<Func<RoomDetails, bool>> expression1 = x => x.Requirement.Gender != null && x.Requirement.Gender == smallExceptions.RHS;
                        //totalExpression = expression1 == null ? totalExpression : AndAlso(totalExpression, expression1);
                        if (expression1 != null && totalExpression != null)
                        {
                            totalExpression = totalExpression.And(expression1);
                        }
                        else if (expression1 != null)
                        {
                            totalExpression = expression1;
                        }
                        
                    }
                    if(smallExceptions.LHS == "location")
                    {
                        List<string> specialtyList = smallExceptions.RHS.Split(",").ToList();
                        Expression<Func<RoomDetails, bool>> expression1 = x => x.Address.State != null && specialtyList.Contains(x.Address.State);
                        //totalExpression = expression1 == null ? totalExpression : AndAlso(totalExpression, expression1);
                        if (expression1 != null && totalExpression != null)
                        {
                            totalExpression = totalExpression.And(expression1);
                        }
                        else if (expression1 != null)
                        {
                            totalExpression = expression1;
                        }
                    }
                    if(smallExceptions.LHS == "userType")
                    {
                        Expression<Func<RoomDetails, bool>> expression2 = x => x.Identity != null && x.Identity == smallExceptions.RHS;
                        //totalExpression = expression1 == null ? totalExpression : AndAlso(totalExpression, expression1);
                        if (expression2 != null && totalExpression != null)
                        {
                            totalExpression = totalExpression.And(expression2);
                        }
                        else if (expression2 != null)
                        {
                            totalExpression = expression2;
                        }
                    }
                }
            }
            
            return totalExpression;
        }

        public async Task<RoomDescription> GetRoomDescriptionById(string filterId)
        {
            //IList<RoomDetails> roomCollection = await this.roomDetailsRepo.FindAllAsync(null);

            IList<RoomDetails> roomCollection = await this.GetRoomDetailsFromMemoryCache(null);
            var roomDetails = roomCollection.First(x=>x.Id == filterId);
            if (roomDetails != null)
            {
                //IList<RoomDescription> rdxCollection = await this.roomDescriptionRepo.FindAllAsync(null);

                IList<RoomDescription> rdxCollection = await this.GetRoomDescriptionFromMemoryCache(null);
                RoomDescription roomDescription = rdxCollection.First(x => x.RoomId == filterId);
                if (roomDescription != null)
                {
                    return roomDescription;    
                }
                else
                {
                    throw new Exception("Room Description not found for the given Id");
                }
                
            }
            else
            {
                throw new Exception("Room Details not present for the gievn Id");
            }
            
      
        }
        public async Task<Boolean> EditRoomCombinedDetails(RoomCombinedDetails roomCombinedDetails)
        {
            //var existingRoomDetailsList = await this.roomDetailsRepo.FindAllAsync(x=>x.Id == roomCombinedDetails.Id);
            //var existingRoomDescriptionList = await this.roomDescriptionRepo.FindAllAsync(x => x.RoomId == roomCombinedDetails.Id);

            var existingRoomDetailsList = await this.GetRoomDetailsFromMemoryCache(x => x.Id == roomCombinedDetails.Id);
            var existingRoomDescriptionList = await this.GetRoomDescriptionFromMemoryCache(x => x.RoomId == roomCombinedDetails.Id);
            var existingRoomDetails = existingRoomDetailsList[0];
            var existingRoomDescription = existingRoomDescriptionList[0];


            existingRoomDetails.Name = roomCombinedDetails.Name;
            existingRoomDetails.Address = roomCombinedDetails.Address;
            existingRoomDetails.Requirement = roomCombinedDetails.Requirement;
            existingRoomDetails.Amount = roomCombinedDetails.Amount;
            existingRoomDetails.Status = roomCombinedDetails.Status;
            //existingRoomDetails.Identity = roomCombinedDetails.RoomType == "Room" ? "Room Mate Finder" : "Room Finder";


            existingRoomDescription.FlatNumber = roomCombinedDetails.FlatNumber;
            existingRoomDescription.AmenityType = roomCombinedDetails.AmenityType;
            existingRoomDescription.Amenities = roomCombinedDetails.Amenities;
            existingRoomDescription.Rent = roomCombinedDetails.Amount;
            existingRoomDescription.Deposit = roomCombinedDetails.Deposit;
            existingRoomDescription.Maintenance = roomCombinedDetails.Maintenance;
            //existingRoomDescription.Gender = roomCombinedDetails.Requirement.Gender;
            existingRoomDescription.PostedOn = roomCombinedDetails.PostedOn;
            existingRoomDescription.AvailableFrom = roomCombinedDetails.AvailableFrom;
            existingRoomDescription.Size = roomCombinedDetails.Requirement.Size;
            existingRoomDescription.State = roomCombinedDetails.Address.State;
            
            //IMongoCollection<RoomDetails> roomDetailsCollection = await this.roomDetailsRepo.getCollection();
            //IMongoCollection<RoomDescription> roomDescriptionCollection = await this.roomDescriptionRepo.getCollection();
            try
            {
                await this.roomDetailsRepo.UpdateAsync(existingRoomDetails);
                //await this.roomDetailsCacheRepo.UpdateMemoryCache("RoomDetails_KEY", existingRoomDetails,x=>x.Id == existingRoomDetails.Id);
                await this.roomDescriptionRepo.UpdateAsync(existingRoomDescription);
                //await this.roomDescriptionCacheRepo.UpdateMemoryCache("RoomDescription_KEY", existingRoomDescription, x => x.RoomId == existingRoomDescription.RoomId);
                //await roomDetailsCollection.ReplaceOneAsync(x => x.Id == roomCombinedDetails.Id, roomDetails);
                //await roomDescriptionCollection.ReplaceOneAsync(x => x.RoomId == roomCombinedDetails.Id, roomDescription);

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return true;

        }

        public async Task<Boolean> DeleteRoomCombinedDetails(string filterId)
        {
            IList<RoomDetails> roomDetailsCollection = await this.roomDetailsRepo.FindAllAsync(x => x.Id == filterId);
            IList<RoomDescription> roomDescriptionCollection = await this.roomDescriptionRepo.FindAllAsync(x => x.RoomId == filterId);
            try
            {
                await this.roomDetailsRepo.DeleteAsync(roomDetailsCollection[0]);
                var modifiedRoomDetails = roomDetailsCollection[0];
                modifiedRoomDetails.Id = "RoomDetails|" + modifiedRoomDetails.Id;
                await this.roomDetailsCacheRepo.DeleteMemoryCache("RoomDetails_KEY", modifiedRoomDetails);
                
   
                await this.roomDescriptionRepo.DeleteAsync(roomDescriptionCollection[0]);
                var modifiedRoomDescription = roomDescriptionCollection[0];
                modifiedRoomDescription.Id = "RoomDescription|" + modifiedRoomDescription.RoomId;
                await this.roomDescriptionCacheRepo.DeleteMemoryCache("RoomDescription_KEY", modifiedRoomDescription);
                IDictionary<string, UserConnection> userConnections = new Dictionary<string, UserConnection>();

                await this.hubContext.Clients.All.SendAsync("ChangedRoomDetails", "Room Details Data updated");
                await this.hubContext.Clients.All.SendAsync("ChangedRoomDescription", "Room Description Data updated");
                
            }
            catch(Exception ex) {
                throw new Exception(ex.Message); 
            }
            return true;
            
        }

        public async Task<List<string>> PostRoomImages(List<IFormFile> files,string folderId)
        {
            if (files == null || files.Count == 0)
            {
                throw new Exception("The file is empty, couldn't upload it");
            }
            try
            {
                var blobStorageConnectionString = this.configuration.GetValue<string>("StorageAccount:ConnectionString");
                var ContainerName = this.configuration.GetValue<string>("StorageAccount:ContainerName");
                BlobServiceClient blobInstance = new BlobServiceClient(blobStorageConnectionString);
                BlobContainerClient blobContainerName = blobInstance.GetBlobContainerClient(ContainerName);

                List<string> blobNames = new List<string>();

                await foreach (BlobItem blobItem in blobContainerName.GetBlobsAsync(prefix: folderId + "/"))
                {
                    blobNames.Add(blobItem.Name);
                }

                for (int i = 0; i < blobNames.Count; i++)
                {
                    BlobClient bc = blobContainerName.GetBlobClient(blobNames[i]);
                    if(await bc.ExistsAsync())
                    {
                        await bc.DeleteAsync();
                    }
                }

                    List<string> BlobUris = new List<string>();
                for (int i = 0; i < files.Count; i++)
                {
                    //creating a new blob name here
                    var fileExt = Path.GetExtension(files[i].FileName).ToLower() == "" ? ".jpg" : Path.GetExtension(files[i].FileName).ToLower();
                    string blobName = folderId + "/"+ Guid.NewGuid().ToString() +"_" + fileExt;
                    //inserting that blob to the container
                    BlobClient blobClient = blobContainerName.GetBlobClient(blobName);

                    using (var stream = files[i].OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream);
                        BlobUris.Add(blobClient.Uri.ToString());
                    }
                }
                return BlobUris;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<List<ImageType>> GetRoomImages(string folderId)
        {
            if (folderId == string.Empty) throw new Exception("No file found in the folder");
            try
            {
                var blobStorageConnectionString = this.configuration.GetValue<string>("StorageAccount:ConnectionString");
                var ContainerName = this.configuration.GetValue<string>("StorageAccount:ContainerName");
                BlobServiceClient blobInstance = new BlobServiceClient(blobStorageConnectionString);
                BlobContainerClient blobContainerName = blobInstance.GetBlobContainerClient(ContainerName);
                List<ImageType> BlobImages = new List<ImageType>();
                List<string> blobNames = new List<string>();

                await foreach (BlobItem blobItem in blobContainerName.GetBlobsAsync(prefix: folderId + "/"))
                {
                    blobNames.Add(blobItem.Name);
                }

                for(int i = 0; i < blobNames.Count; i++)
                {
                    BlobClient bc = blobContainerName.GetBlobClient(blobNames[i]);
                    if(await bc.ExistsAsync())
                    {
                        var blob = bc.Download();
                        Stream data = blob.Value.Content;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            data.CopyTo(ms);
                            byte[] byteArray = ms.ToArray();
                            // Convert byte array to Base64 string

                            var extension = Path.GetExtension(blobNames[i]).ToLower();

                            // You can also include the MIME type in the response
                            var mimeTypeData = "image/jpeg"; // Or determine based on the blob type (e.g., image/png)
                            BlobImages.Add(
                                new ImageType()
                                {
                                    Base64StringData = Convert.ToBase64String(byteArray),
                                    MimeType = extension switch
                                    {
                                        ".svg" => "image/svg",
                                        
                                        ".jpg" or ".jpeg" or ".jfif" => "image/jpeg",
                                        ".png" => "image/png",
                                        ".gif" => "image/gif",
                                        ".bmp" => "image/bmp",
                                        _ => "application/octet-stream", // Default for unknown formats
                                    },
                                    imageName = blobNames[i],
                                });
                           
                        }
                        
                    }
                }
                return BlobImages;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<List<string>> GetMatchedRoomsOrRoomMates(string currentUserName)
        {
            var matchedUserRoomDetails = await this.roomDetailsRepo.FindAllAsync(null);
            var matchedUserNames = matchedUserRoomDetails.Where(x=>x.AssignedTo.Contains(currentUserName)).Select(x => x.Name).Distinct().ToList();
            return matchedUserNames;
        }
            
        public async Task<Boolean> MatchRoomRoomMate(string ID,string fromUser)
        {
            var filteredRoomDetails = await this.roomDetailsRepo.FindAllAsync(x => x.Id == ID);
            var currentRoomDetails = filteredRoomDetails.First();
            if (currentRoomDetails.AssignedTo == null)
            {
                currentRoomDetails.AssignedTo = new List<string> { fromUser };
            }
            else
            {
                var current_Assigned_To = currentRoomDetails.AssignedTo;
                current_Assigned_To.Add(fromUser);
                currentRoomDetails.AssignedTo = current_Assigned_To;
                
            }
            if(currentRoomDetails.Identity == "Room Mate Finder")
            {
                currentRoomDetails.Requirement.Vacancy = currentRoomDetails.Requirement.Vacancy - 1;
                if(currentRoomDetails.Requirement.Vacancy == 0)
                {
                    currentRoomDetails.Status = "Occupied";
                }
            }
            else
            {
                currentRoomDetails.Status = "Occupied";
            }
            
            await this.roomDetailsRepo.UpdateAsync(currentRoomDetails);
            return true;
        }
    }
}
