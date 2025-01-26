namespace Events.Application.Exceptions
{
    public class ServiceException:Exception
    {
        public ServiceException(string message, Exception innerException = null!)
        : base(message, innerException)
        {
        }
    }
}
