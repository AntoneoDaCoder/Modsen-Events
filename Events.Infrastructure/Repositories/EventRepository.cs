using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Events.Core.Abstractions;
using Events.Core.Models;
using Events.Infrastructure.DbContexts;
using Events.Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace Events.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EventsDbContext _dbContext;
        private readonly IMapper _mapper;
        public EventRepository(EventsDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<List<Event>> GetPagedAsync(int index, int pageSize)
        {
            var eventEntities = await _dbContext.Events.AsNoTracking().Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
            return _mapper.Map<List<Event>>(eventEntities);
        }
        public async Task<Event?> GetByIdAsync(Guid id)
        {
            var eEntity = await _dbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (eEntity is not null)
                return _mapper.Map<Event>(eEntity);
            return null;
        }
        public async Task<Event?> GetByNameAsync(string name)
        {
            var eEntity = await _dbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Name == name);
            if (eEntity is not null)
                return _mapper.Map<Event>(eEntity);
            return null;
        }
        public async Task<(bool, IEnumerable<string>)> CreateAsync(Event ev)
        {
            var eEntity = _mapper.Map<EventEntity>(ev);
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
                _mapper.Map(ev, eEntity);
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
                _dbContext.Events.Remove(eEntity);
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
        public async Task<List<Event>> GetByCriterionAsync(Expression<Func<Event, bool>> filter, int index, int pageSize)
        {
            IQueryable<EventEntity> query = _dbContext.Events;
            var entityFilter = _mapper.MapExpression<Expression<Func<EventEntity, bool>>>(filter);
            query = query.Where(entityFilter);
            var eventEntities = await query.AsNoTracking().Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
            return _mapper.Map<List<Event>>(eventEntities);
        }
    }
}

