using Events.Core.Contracts;
namespace Events.Core.Abstractions
{
    public interface IEventParticipantRepository
    {
        Task<(bool, IEnumerable<string>)> RegisterParticipantAsync(Guid eventId, string participantId);
        Task<List<ParticipantWithDateDTO>> GetPagedParticipantsAsync(Guid eventId, int index, int pageSize);
        Task<(bool, IEnumerable<string>)> UnregisterParticipantAsync(Guid eventId, string participantId);
        Task<ParticipantWithDateDTO?> GetEventParticipantByIdAsync(Guid eventId, string participantId);
    }
}
