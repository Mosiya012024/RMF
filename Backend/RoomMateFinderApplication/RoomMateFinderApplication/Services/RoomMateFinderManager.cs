using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RoomMateFinderApplication.Models;
using RoomMateFinderApplication.Repositories;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using Expression = System.Linq.Expressions.Expression;

namespace RoomMateFinderApplication.Services
{
    [ExcludeFromCodeCoverage]
    public class RoomMateFinderManager : IRoomMateFinderManager
    {
        private readonly IRepository<RoomDetails> repo;
        private readonly IRepository<RoomDescription> roomDescriptionRepo;
        private readonly List<Address> possibleAddresses;
        public readonly MongoDbService mongoDbService;

        public RoomMateFinderManager(IRepository<RoomDetails> repo, MongoDbService mongoDbService, IRepository<RoomDescription> roomDescriptionRepo)
        {
            this.repo = repo;
            AddressDuplicateData AddressGenerator = new AddressDuplicateData();
            possibleAddresses = AddressGenerator.getDuplicateAddresses();
            this.mongoDbService = mongoDbService;
            this.roomDescriptionRepo = roomDescriptionRepo;
        }

        public List<RoomDetails> GetRoomsOrderBy(string orderBy,List<RoomDetails> allRooms)
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
        public async Task<List<RoomDetails>> GetAllRooms(string filter,string orderBy)
        {
            //actual code 
            //var rooms = await this.repo.FindAllAsync();


            //will remove this code afterwards
            //var allRooms =  await mongoDbService.GetUsersAsync(filterPredicate);
            //return allRooms;
            var filterPredicate = this.FindFilterExpression(filter);
            IMongoCollection<RoomDetails> collection = await this.repo.getCollection();
            var allRooms = filterPredicate == null ? collection.Find(x=>true).ToList() : await collection.Find(filterPredicate).ToListAsync();
            var sortedRooms = GetRoomsOrderBy(orderBy,allRooms);
            return sortedRooms;
        }

        public async Task<RoomDetails> PostRoom(RoomDetails roomDetails)
        {
            roomDetails.Id = ObjectId.GenerateNewId().ToString();
            this.repo.CreateAsync(roomDetails);
            return roomDetails;
        }

        public async void PostRoomCombinedDetails(RoomCombinedDetails roomCombinedDetails)
        {
            RoomDetails roomDetails = new RoomDetails()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = roomCombinedDetails.Name,
                Address = roomCombinedDetails.Address,
                Requirement = roomCombinedDetails.Requirement,
                Amount = roomCombinedDetails.Amount,
                Status = roomCombinedDetails.Status,
                Identity = roomCombinedDetails.RoomType == "Room" ? "Room Mate Finder" : "Room Finder",
            };
            this.repo.CreateAsync(roomDetails);
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
            this.roomDescriptionRepo.CreateAsync(roomDescription);

        }

        public async Task<List<RoomDetails>> PostMultipleRooms(int number)
        {
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
            List<string> Occupancy = new List<string>() { "Single", "2 - Shared", "3 - Shared", "4 - Shared" };
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
                    Id = ObjectId.GenerateNewId().ToString(),
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
                    Identity = userIdentities[rand.Next(0,userIdentities.Count)],

                };
                
                room.SetRequirementIdentity();
                roomsAdded.Add(room);
                RoomDescription rd = new RoomDescription()
                {
                    RoomId = room.Id,
                    FlatNumber = room.Identity.Equals("Room Mate Finder") ? ("Flat No " + flatNumbers[rand.Next(0, flatNumbers.Count)] + " " + room.Address.Area + " " + room.Address.City + " " + room.Address.State) : room.Address.Area +", "+ room.Address.City +", " + room.Address.State,
                    AmenityType = AmenitiesType[rand.Next(0, AmenitiesType.Count)],
                    //AmenityType = "Furnished",
                    Deposit = rand.Next(12, 21) * 60,
                    Maintenance = room.Identity.Equals("Room Mate Finder") ? rand.Next(12, 21) * 6 : 0,
                    PostedOn = DateTime.Now,
                    AvailableFrom = DateTime.Now.AddDays(availableFromDates[rand.Next(0, availableFromDates.Count)]),
                    State = room.Address.State,
                    Gender = room.Requirement.Gender,
                    Rent = room.Amount,
                    Size = room.Identity.Equals("Room Mate Finder") ? room.Requirement.Size : Occupancy[rand.Next(0,Occupancy.Count)],
                };
                // this.roomDescriptionRepo.CreateAsync(rd);
                roomDescriptionsAdded.Add(rd);
                number--;
            }
            await this.roomDescriptionRepo.CreateMultipleAsync(roomDescriptionsAdded);

            var allRooms = await this.repo.CreateMultipleAsync(roomsAdded);
            return allRooms;
        }

        public async Task<List<RoomDetails>> DeleteMultipleRooms(int number)
        {
            var roomsRemaining = await this.repo.DeleteMultipleAsync(number);
            return roomsRemaining;
        }

        private Models.Expression FindInnerExpressions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter) || string.IsNullOrEmpty(filter))
            {
                return null;
            }
            Models.Expression filterExpression = new Models.Expression
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
            IMongoCollection<RoomDetails> roomCollection = await this.repo.getCollection();
            var roomDetails = roomCollection.Find(x=> x.Id == filterId).First();
            if (roomDetails != null)
            {
                IMongoCollection<RoomDescription> rdxCollection = await this.roomDescriptionRepo.getCollection();
                RoomDescription roomDescription = rdxCollection?.Find(x => x.RoomId == filterId).First();
                if (roomDescription != null)
                {
                    return roomDescription;    
                }
                else
                {
                    throw new Exception("Room Description not found for the gievn Id");
                }
                
            }
            else
            {
                throw new Exception("Room Details not present for the gievn Id");
            }
            
      
        }
        public async void EditRoomCombinedDetails(RoomCombinedDetails roomCombinedDetails)
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
            };
            RoomDescription roomDescription = new RoomDescription()
            {
                RoomId = roomCombinedDetails.Id,
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
            IMongoCollection<RoomDetails> roomDetailsCollection = await this.repo.getCollection();
            IMongoCollection<RoomDescription> roomDescriptionCollection = await this.roomDescriptionRepo.getCollection();
            try
            {
                await roomDetailsCollection.ReplaceOneAsync(x => x.Id == roomCombinedDetails.Id, roomDetails);
                await roomDescriptionCollection.ReplaceOneAsync(x => x.RoomId == roomCombinedDetails.Id, roomDescription);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }


        }

        public async void DeleteRoomCombinedDetails(string filterId)
        {
            IMongoCollection<RoomDetails> roomDetailsCollection = await this.repo.getCollection();
            IMongoCollection<RoomDescription> roomDescriptionCollection = await this.roomDescriptionRepo.getCollection();
            try
            {
                await roomDetailsCollection.DeleteOneAsync(x => x.Id == filterId);
                await roomDescriptionCollection.DeleteOneAsync(x =>x.RoomId == filterId);
            }
            catch(Exception ex) {
                throw new Exception(ex.Message); 
            }
            
        }
    }
}
