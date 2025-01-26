namespace Events.Application.Exceptions
{
    public class BusinessModelException : Exception
    {
        public BusinessModelException(string message, Exception innerException = null!)
        : base(message, innerException)
        {
        }
    }
}
