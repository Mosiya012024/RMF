using Azure_Room_Mate_Finder.Model;
using Azure_Room_Mate_Finder.Repository;
using Azure_Room_Mate_Finder.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Configuration;

namespace Azure_Room_Mate_Finder.Configuration
{
    public class NotificationHubProxy:Hub
    {
        private readonly user_conn_ids_dict dict_user_conn_ids;

        //private readonly IDictionary<string, string> dict_user_conn_ids;
        private readonly username_chatroom_dict dict_username_chatRoom;
        private readonly users_dict dict_users;
        private readonly ICosmosDbRepository<ChatMessage> cosmosDbRepository;
        //private readonly IDictionary<string, string> dict_chat_room_user_name;
        private readonly chat_room_user_name_dict dict_chat_room_user_names;

        public NotificationHubProxy(ICosmosDbRepository<ChatMessage> cosmosDbRepository,user_conn_ids_dict dict_users_conn_ids,users_dict connection, username_chatroom_dict dict_username_chatRoom
            , chat_room_user_name_dict dict_chat_room_user_names
            ) {
            this.cosmosDbRepository = cosmosDbRepository;
            this.dict_user_conn_ids = dict_users_conn_ids;
            dict_users = connection;
            this.dict_username_chatRoom = dict_username_chatRoom;
            this.dict_chat_room_user_names = dict_chat_room_user_names;
            
        }

        public override async Task OnConnectedAsync()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            dict_user_conn_ids.TryGetValue(userName, out string OriginalconnID);


            if (dict_user_conn_ids.ContainsKey(userName))
            {
                dict_user_conn_ids[userName] = connectionId;
            }
            else
            {
                dict_user_conn_ids.Add(userName, connectionId);
            }

            if(dict_username_chatRoom.TryGetValue(userName,out UserConnection connn)) {
                dict_users.Remove(OriginalconnID);
                dict_users[connectionId] = connn;
                await Groups.AddToGroupAsync(connectionId, groupName: connn.ChatRoom);
                await SendConnectedUser(connn.ChatRoom);
                var formattedAllMessages = await this.GetChatMessagesAsync(connn);
                await Clients.All.SendAsync(method: "RecieveMessage", formattedAllMessages);
               
            }
            
            await base.OnConnectedAsync();
        }
        
        public async Task SendChatRequest(string toUser,string fromUser)
        {
            if(dict_user_conn_ids.TryGetValue(toUser, out string connectionID))
            {
                await Clients.Client(connectionID).SendAsync(method: "SendChatRequestMethodResonse", fromUser, "Accept Request", "Reject Request");
            }
        }

        public async Task AcceptChatRequest(string fromUser)
        {
            if(dict_user_conn_ids.TryGetValue(fromUser, out string connectionID)) {
                await Clients.Client(connectionID).SendAsync(method: "AcceptChatRequestMethodResponse", Context.User.Identity.Name + " has accepted your chat request.");
            }
        }

        public async Task RejectChatRequest(string fromUser)
        {
            if (dict_user_conn_ids.TryGetValue(fromUser, out string connectionID))
            {
                await Clients.Client(connectionID).SendAsync(method: "RejectChatRequestMethodResponse", Context.User.Identity.Name + " has rejected your chat request.");
            }
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
                dict_username_chatRoom.Add(user.UserName, user);
                if(dict_chat_room_user_names.ContainsKey(user.ChatRoom))
                {
                    dict_chat_room_user_names.TryGetValue(user.ChatRoom, out List<string> user_names);
                    user_names.Add(user.UserName);
                    dict_chat_room_user_names[user.ChatRoom] = user_names;
                }
                else
                {
                    dict_chat_room_user_names.Add(user.ChatRoom, new List<string>() { user.UserName });
                }
                
            }
            else if(connect != null && connect.ChatRoom != user.ChatRoom) //if user wants to chat with multiple users,like joining multiple rooms using same connectionID
            {
                //to create a connection to the group
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName: user.ChatRoom);
                dict_users[Context.ConnectionId] =  user;
                dict_username_chatRoom[user.UserName] = user;
                if (dict_chat_room_user_names.ContainsKey(user.ChatRoom))
                {
                    dict_chat_room_user_names.TryGetValue(user.ChatRoom, out List<string> user_names);
                    user_names.Add(user.UserName);
                    dict_chat_room_user_names[user.ChatRoom] = user_names;
                }
                else
                {
                    dict_chat_room_user_names.Add(user.ChatRoom, new List<string>() { user.UserName });
                }
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

                dict_username_chatRoom.Remove(user.UserName);
                // Send updated user list
                await SendConnectedUser(chatRoom);
                await Clients.Group(user.ChatRoom).SendAsync("LeaveRoomMessage", $"{user.UserName} has left the chat", formattedDate);
                // Remove the user from the group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoom);




                // Notify others in the group

            }
        }

        //public async Task LeaveRoom(string chatRoom)
        //{
        //    if (dict_users.TryGetValue(Context.ConnectionId, out UserConnection user) && user.ChatRoom == chatRoom)
        //    {
        //        string formattedDate = DateTime.Now.ToString("MMM dd, yyyy, hh:mm:ss tt");


        //        // Remove the user from the dictionary
        //        dict_users.Remove(Context.ConnectionId);

        //        dict_username_chatRoom.Remove(user.UserName);
        //        // Send updated user list
        //        await SendConnectedUser(chatRoom);
        //        await Clients.Group(user.ChatRoom).SendAsync("LeaveRoomMessage", $"{user.UserName} has left the chat", formattedDate);
        //        // Remove the user from the group
        //        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoom);




        //        // Notify others in the group

        //    }
        //}

        public async Task CheckJoinedFirstTime(string currentUser,string chatRoom)
        {
            if (dict_chat_room_user_names.ContainsKey(chatRoom))
            {
                //dict_user_conn_ids.TryGetValue(toUser, out string connectionID);
                dict_chat_room_user_names.TryGetValue(chatRoom, out List<string> usernames);
                var check = usernames.Contains(currentUser);
                await Clients.Client(Context.ConnectionId).SendAsync("IsFirstJoined", !check);
            }
            else
            {
                //dict_user_conn_ids.TryGetValue(toUser, out string connectionID);
                var check = dict_chat_room_user_names.ContainsKey(chatRoom);
                await Clients.Client(Context.ConnectionId).SendAsync("IsFirstJoined", !check);
            }
        }

        public async Task SendBookRequest(string toUser,string fromUser,string toBeMatchedID)
        {
            if(dict_user_conn_ids.TryGetValue(toUser, out string connID))
            {
                await Clients.Client(connID).SendAsync("AcceptOrRejectBookRequest", "A request from " + fromUser + "to book the room/room mate", fromUser, toBeMatchedID);
            }
        }

        public async Task AcceptedBookRequest(string fromUser_Name, string toBeMatched)
        {
            if(dict_user_conn_ids.TryGetValue(fromUser_Name, out string connID))
            {
                await Clients.Client(connID).SendAsync("ToUserAccepted", "Its a Match", toBeMatched);
            }
        }

    }
}
