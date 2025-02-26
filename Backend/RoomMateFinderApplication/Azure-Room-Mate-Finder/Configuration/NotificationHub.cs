using Azure_Room_Mate_Finder.Model;
using Azure_Room_Mate_Finder.Repository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;
using System.Threading;

namespace Azure_Room_Mate_Finder.Configuration
{
    public class NotificationHub : Hub
    {

        private readonly IDictionary<string, UserConnection> dict_users;
        private readonly ICosmosDbRepository<ChatMessage> cosmosDbRepository;

        public NotificationHub(IDictionary<string, UserConnection> connection, ICosmosDbRepository<ChatMessage> cosmosDbRepository)
        {
            dict_users = connection;
            this.cosmosDbRepository = cosmosDbRepository;
        }

        public async Task NotifyChange(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        //to join the chat and send the message to all people in the application
        public async Task JoinChat(UserConnection conn)
        {
            await Clients.All.SendAsync(method: "JoinChatRecieveMesssage", arg1: "admin", arg2: conn.UserName + "has joined");
           
        }

        //user can join a specific chat group and send the message to a particular group
        public async Task JoinRoom(UserConnection user)
        {
            
            dict_users.TryGetValue(Context.ConnectionId, out UserConnection connect);
            if (connect == null)
            {
                //to create a connection to the group
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName: user.ChatRoom);
               
                //adding the users who connected to the group
                dict_users.Add(Context.ConnectionId, user);
                
            }
            DateTime CurrentTime = DateTime.Now;
            string formattedDate = CurrentTime.ToString("MMM dd, yyyy, hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
            Console.WriteLine(formattedDate);

            //to invoke the methods of the group to say that particular user has joined the group
            await Clients.Group(user.ChatRoom).SendAsync(method: "JoinRoomMessage", user.UserName + " has joined the chat", formattedDate);
            await SendConnectedUser(user.ChatRoom);
            var formattedAllMessages = await this.GetChatMessagesAsync(user);
            await Clients.All.SendAsync(method: "RecieveMessage", formattedAllMessages);
        }

        //sending the message to particular room that the user has types 
        public async Task SendMessage(string Message)
        {
            dict_users.TryGetValue(Context.ConnectionId, out UserConnection user);
            DateTime CurrentTime = DateTime.Now;
            string formattedDate = CurrentTime.ToString("MMM dd, yyyy, hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
            Console.WriteLine(formattedDate);
            var chatMessage = new ChatMessage()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = user.UserName,
                ChatRoom = user.ChatRoom,
                Message = Message,
                MessageTimeStamp = DateTime.Now,
                ActualMessage = false,
            };
            await this.cosmosDbRepository.PostAsync(chatMessage);

            var formattedAllMessages = await this.GetChatMessagesAsync(user);

            await Clients.Groups(user.ChatRoom).SendAsync(method: "RecieveMessage", formattedAllMessages);
        }

        public async Task<List<object>> GetChatMessagesAsync(UserConnection user)
        {
            var allMessages = await this.cosmosDbRepository.FindAllAsync(null);
            var formattedAllMessages = allMessages.Where(x => x.ChatRoom == user.ChatRoom).OrderBy(x => x.MessageTimeStamp)
                .Select(x => new
                {
                    x.Id,
                    x.UserName,
                    x.ChatRoom,
                    x.Message,
                    x.ActualMessage,
                    FormattedMessageTimeStamp = x.MessageTimeStamp.ToString("MMM dd, yyyy, hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture)
                }).ToList<object>();
            return formattedAllMessages;
        }

        //to display how many people are connected to a room
        public Task SendConnectedUser(string room)
        {
            var users = dict_users.Values.Where(x => x.ChatRoom == room).Select(x => x.UserName);
            return Clients.Group(room).SendAsync(method: "ConnectedUser", users);
        }

        public async Task LeaveRoom(string chatRoom)
        {
            if (dict_users.TryGetValue(Context.ConnectionId, out UserConnection user) && user.ChatRoom == chatRoom)
            {
                string formattedDate = DateTime.Now.ToString("MMM dd, yyyy, hh:mm:ss tt");
                

                // Remove the user from the dictionary
                dict_users.Remove(Context.ConnectionId);
                // Send updated user list
                await SendConnectedUser(chatRoom);
                await Clients.Group(user.ChatRoom).SendAsync("LeaveRoomMessage", $"{user.UserName} has left the chat", formattedDate);
                // Remove the user from the group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoom);

                
                

                // Notify others in the group
                
            }
        }

        //to know if any user has left the chat room
        //public override Task OnDisconnectedAsync(Exception? exception)
        //{
        //    if (!dict_users.TryGetValue(Context.ConnectionId, out UserConnection user))
        //    {
        //        OnDisconnectedAsync(exception);
        //    }
        //    DateTime CurrentTime = DateTime.Now;
        //    string formattedDate = CurrentTime.ToString("MMM dd, yyyy, hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);

        //    Clients.Group(user.ChatRoom).SendAsync(method: "LeaveRoomMessage",user.UserName + " has left the group", formattedDate);

        //    SendConnectedUser(user.ChatRoom);
        //    return OnDisconnectedAsync(exception);
        //}


    }
}
