
namespace Events.Application.Exceptions
{
    public class IncorrectDataException : EventsException
    {
        public IncorrectDataException(string message, Exception? inner = null) : base(message, inner)
        {
        }
    }
}
