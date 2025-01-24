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
        public ICollection<Participant> Participants { get; } = new List<Participant>();
        private Event
            (Guid id, string name, string description, DateOnly date, TimeOnly time,
            string location, string category, uint maxParticipants, ICollection<Participant> participants)
        {
            Id = id;
            Name = name;
            Description = description;
            Date = date;
            Time = time;
            Location = location;
            Category = category;
            MaxParticipants = maxParticipants;
            Participants = participants;
        }
        static public Event CreateEvent(Guid id, string name, string description, DateOnly date, TimeOnly time,
            string location, string category, uint maxParticipants, ICollection<Participant> participants)
        {
            return new Event(id, name, description, date, time, location, category, maxParticipants, participants);
        }
    }
}
