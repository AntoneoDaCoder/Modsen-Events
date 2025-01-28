using System.Text;

namespace Events.Application.Exceptions
{
    public class ServiceException : EventsException
    {
        public ServiceException(string message, Exception? inner = null) : base(message, inner)
        {
        }
    }
}
