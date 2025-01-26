namespace Events.Application.Contracts
{
    public record RegisterParticipantDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; } = DateOnly.MinValue;
        public string Email { get; set; } = string.Empty;
    }
}
