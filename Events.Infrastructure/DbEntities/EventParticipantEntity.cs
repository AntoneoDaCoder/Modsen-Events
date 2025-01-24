namespace Events.Infrastructure.DbEntities
{
    public class EventParticipantEntity
    {
        public Guid EventId { get; set; }
        public EventEntity Event { get; set; } = null!;
        public Guid ParticipantId { get; set; }
        public ParticipantEntity Participant { get; set; } = null!;
        public DateOnly RegisterDate { get; set; } = DateOnly.MinValue;
    }
}
