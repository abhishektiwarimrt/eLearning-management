namespace lms.usermanagement.api.Exceptions
{
    public class UserProfileNotFoundException : Exception
    {
        public UserProfileNotFoundException() : base("User Profile Not Found!")
        {

        }
    }
}
