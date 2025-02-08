
namespace Events.Infrastructure.DbEntities
{
    public class EventEntity
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly Date { get; set; } = DateOnly.MinValue;
        public TimeOnly Time { get; set; } = TimeOnly.MinValue;
        public string Location { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public uint MaxParticipants { get; set; } = 0;
        public string ImagePath { get; set; } = string.Empty;
        public ICollection<EventParticipantEntity> EventParticipants { get; set; } = new List<EventParticipantEntity>();
    }
}
