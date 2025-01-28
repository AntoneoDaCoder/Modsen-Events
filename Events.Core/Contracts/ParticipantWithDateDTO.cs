using Events.Core.Models;
namespace Events.Core.Contracts
{
    public record ParticipantWithDateDTO
    {
        public Participant Participant { get; set; } = null!;
        public DateOnly RegisterDate { get; set; }
    }
}
