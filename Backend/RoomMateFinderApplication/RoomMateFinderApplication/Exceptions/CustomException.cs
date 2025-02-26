namespace RoomMateFinderApplication.Exceptions
{
    public class CustomException : Exception
    {
        public override string Message
        {
            get
            {
                return "Not enough data to delete";
            }
        }
    }
}
