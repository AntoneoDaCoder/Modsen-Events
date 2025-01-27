namespace Events.Infrastructure.DbEntities
{
    public class EventParticipantEntity
    {
        public Guid EventId { get; set; }
        public EventEntity Event { get; set; } = null!;
        public string ParticipantId { get; set; } = string.Empty;
        public ParticipantEntity Participant { get; set; } = null!;
        public DateOnly RegisterDate { get; set; } = DateOnly.MinValue;
    }
}
