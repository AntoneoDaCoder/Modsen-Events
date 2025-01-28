namespace Events.Application.Contracts
{
    public record AddEventParticipantDTO
    {
        public string ParticipantId { get; set; } = string.Empty;
    }
}
