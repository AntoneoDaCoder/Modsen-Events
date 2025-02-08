using Events.Core.Contracts;
namespace Events.Core.Abstractions
{
    public interface IEventParticipantRepository
    {
        Task RegisterParticipantAsync(Guid eventId, string participantId);
        Task<List<ParticipantWithDateDTO>> GetPagedParticipantsAsync(Guid eventId, int index, int pageSize);
        Task UnregisterParticipantAsync(Guid eventId, string participantId, DateOnly regDate);
        Task<ParticipantWithDateDTO?> GetEventParticipantByIdAsync(Guid eventId, string participantId);
    }
}
