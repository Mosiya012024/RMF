

using System.Diagnostics.Eventing.Reader;

namespace Azure_Room_Mate_Finder.Model
{
    public class User
    {
        public string Usertype { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public bool isConfirmed { get; set; }

        public string Token { get; set; }

        public string MfaSecret { get; set; }
    }
}
