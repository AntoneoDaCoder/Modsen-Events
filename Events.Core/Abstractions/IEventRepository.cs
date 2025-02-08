using Events.Core.Models;
using System.Linq.Expressions;
namespace Events.Core.Abstractions
{
    public interface IEventRepository
    {
        Task<List<Event>> GetPagedAsync(int index, int pageSize);
        Task<Event?> GetByIdAsync(Guid id);
        Task<(Event?, int?)> GetByIdWithParticipantsAsync(Guid id);
        Task<Event?> GetByNameAsync(string name);
        Task CreateAsync(Event ev);
        Task UpdateAsync(Event target, Event ev);
        Task DeleteAsync(Event target);
        Task<List<Event>> GetByCriterionAsync(Expression<Func<Event, bool>> filter, int index, int pageSize);
    }
}
