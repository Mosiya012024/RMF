namespace Azure_Room_Mate_Finder.Model
{
    public class ChatMessage
    {
        public string Id { get; set; } = string.Empty;
        public string ChatRoom { get; set; }
        public string UserName { get; set; }

        public string Message { get; set; }

        public DateTime MessageTimeStamp { get; set; }

        public bool ActualMessage { get; set; }

    }
}
