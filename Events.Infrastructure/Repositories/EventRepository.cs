using Events.Core.Abstractions;
using Events.Core.Models;
using Events.Infrastructure.DbContexts;
using Events.Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;
namespace Events.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EventsDbContext _dbContext;
        public EventRepository(EventsDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Event>> GetPagedAsync(int index, int pageSize)
        {
            var eventEntities = await _dbContext.Events.AsNoTracking().Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
            return eventEntities.Select(e => Event.CreateEvent(e.Id, e.Name, e.Description, e.Date, e.Time, e.Location, e.Category, e.MaxParticipants)).ToList();
        }
        public async Task<Event?> GetByIdAsync(Guid id)
        {
            var eEntity = await _dbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (eEntity is not null)
                return Event.CreateEvent(eEntity.Id, eEntity.Name, eEntity.Description, eEntity.Date, eEntity.Time, eEntity.Location, eEntity.Category, eEntity.MaxParticipants);
            return null;
        }
        public async Task<Event?> GetByNameAsync(string name)
        {
            var eEntity = await _dbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Name == name);
            if (eEntity is not null)
                return Event.CreateEvent(eEntity.Id, eEntity.Name, eEntity.Description, eEntity.Date, eEntity.Time, eEntity.Location, eEntity.Category, eEntity.MaxParticipants);
            return null;
        }
        public async Task<(bool, IEnumerable<string>)> CreateAsync(Event ev)
        {
            //auto mapper here
            var eEntity = new EventEntity()
            {
                Id = ev.Id,
                Name = ev.Name,
                Description = ev.Description,
                Date = ev.Date,
                Time = ev.Time,
                Location = ev.Location,
                Category = ev.Category,
                MaxParticipants = ev.MaxParticipants
            };
            try
            {
                _dbContext.Events.Add(eEntity);
                int res = await _dbContext.SaveChangesAsync();
                return (res > 0, Enumerable.Empty<string>());
            }
            catch (DbUpdateException ex)
            {
                var errors = new List<string> { ex.Message };
                if (ex.InnerException != null)
                {
                    errors.Add(ex.InnerException.Message);
                }
                return (false, errors);
            }
            catch (Exception ex)
            {
                return (false, new List<string> { ex.Message });
            }
        }
        public async Task<(bool, IEnumerable<string>)> UpdateAsync(Guid id, Event ev)
        {
            var eEntity = await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (eEntity is not null)
            {
                eEntity.Name = ev.Name;
                eEntity.Description = ev.Description;
                eEntity.Date = ev.Date;
                eEntity.Time = ev.Time;
                eEntity.Location = ev.Location;
                eEntity.Category = ev.Category;
                eEntity.MaxParticipants = ev.MaxParticipants;
                try
                {
                    int res = await _dbContext.SaveChangesAsync();
                    return (res > 0, Enumerable.Empty<string>());
                }
                catch (DbUpdateException ex)
                {
                    var errors = new List<string> { ex.Message };
                    if (ex.InnerException != null)
                    {
                        errors.Add(ex.InnerException.Message);
                    }
                    return (false, errors);
                }
                catch (Exception ex)
                {
                    return (false, new List<string> { ex.Message });
                }
            }
            return (false, new List<string> { "Event not found" });
        }
        public async Task<(bool, IEnumerable<string>)> DeleteAsync(Guid id)
        {
            var eEntity = await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (eEntity is not null)
            {

            }
            return (false, new List<string> { "Event not found" });
        }
    }
}

