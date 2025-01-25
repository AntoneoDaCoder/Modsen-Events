using Events.Core.Models;

namespace Events.Core.Abstractions
{
    public interface IEventParticipantRepository
    {
        Task<(bool, IEnumerable<string>)> RegisterParticipantAsync(Guid eventId, Guid participantId);
        Task<(List<Participant>, IEnumerable<string>)> GetPagedParticipantsAsync(Guid eventId, int index, int pageSize);
        Task<(bool, IEnumerable<string>)> UnregisterParticipantAsync(Guid eventId, Guid participantId);
    }
}
