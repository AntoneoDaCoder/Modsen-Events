using Events.Core.Models;
using System.Linq.Expressions;
namespace Events.Core.Abstractions
{
    public interface IEventRepository
    {
        Task<List<Event>> GetPagedAsync(int index, int pageSize);
        Task<Event?> GetByIdAsync(Guid id);
        Task<Event?> GetByNameAsync(string name);
        Task<(bool, IEnumerable<string>)> CreateAsync(Event ev);
        Task<(bool, IEnumerable<string>)> UpdateAsync(Guid id, Event ev);
        Task<(bool, IEnumerable<string>)> DeleteAsync(Guid id);
        Task<List<Event>> GetByCriterionAsync(Expression<Func<Event, bool>> filter,int index,int pageSize);
    }
}
