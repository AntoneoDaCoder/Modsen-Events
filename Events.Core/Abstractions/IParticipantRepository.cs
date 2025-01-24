using Events.Core.Models;
namespace Events.Core.Abstractions
{
    public interface IParticipantRepository
    {
        Task<List<Participant>> GetAsync(Guid? id);
        Task<(bool, IEnumerable<string>)> CreateAsync(Participant participant, string password);
        Task<(bool, IEnumerable<string>)> UpdateAsync(Guid id, Participant participant);
        Task<(bool, IEnumerable<string>)> DeleteAsync(Guid id);
    }
}
