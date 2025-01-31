using System.Text.Json.Serialization;
namespace Events.Core.Models
{
    public class Event
    {
        public Guid Id { get; }
        public string Name { get; } = string.Empty;
        public string Description { get; } = string.Empty;
        public DateOnly Date { get; } = DateOnly.MinValue;
        public TimeOnly Time { get; } = TimeOnly.MinValue;
        public string Location { get; } = string.Empty;
        public string Category { get; } = string.Empty;
        public uint MaxParticipants { get; } = 0;
        public string ImagePath { get; private set; } = string.Empty;
        private Event
            (Guid id, string name, string description, DateOnly date, TimeOnly time,
            string location, string category, uint maxParticipants)
        {
            Id = id;
            Name = name;
            Description = description;
            Date = date;
            Time = time;
            Location = location;
            Category = category;
            MaxParticipants = maxParticipants;
        }
        static public Event CreateEvent(Guid id, string name, string description, DateOnly date, TimeOnly time,
            string location, string category, uint maxParticipants)
        {
            return new Event(id, name, description, date, time, location, category, maxParticipants);
        }
        public void AttachImage(string path)
        {
            ImagePath = path;
        }
    }
}
