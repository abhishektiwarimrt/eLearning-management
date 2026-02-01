namespace lms.services.usermanagement.Exceptions
{
    public class RegistrationFailedException : InternalServerException
    {
        public RegistrationFailedException(string message) : base(message)
        {
        }

        public RegistrationFailedException(string message, string details) : base(message)
        {
            Details = details;
        }

        public new string? Details { get; }
    }
}
