namespace Azure_Room_Mate_Finder.Model
{
    public class SmtpSettings
    {
        public string Server { get; set; }

        public int Port { get; set; }

        public string SenderName { get; set; }

        public string SenderEmail { get; set; }

        public string ToEmail { get; set; }

        public string UserName { get; set; }

        public string Password { get; set;}

        public bool EnableSsl {  get; set; }
    }
}
