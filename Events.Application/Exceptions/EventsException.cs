using System.Text;
namespace Events.Application.Exceptions
{
    public class EventsException : Exception
    {
        public EventsException(string message, Exception? inner = null) : base(message, inner)
        {
        }
        public static T RaiseException<T>(string desc, IEnumerable<string> errors, Exception? inner = null) where T : EventsException
        {
            StringBuilder sb = new StringBuilder();
            foreach (var error in errors)
                sb.Append("[" + error + "], ");
            sb.Remove(sb.Length - 2, 2);
            return (T)Activator.CreateInstance(typeof(T), desc + ": " + sb.ToString(), inner)!;
        }
    }
}
