using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Events.Core.Abstractions;
using Events.Core.Models;
using Events.Infrastructure.DbContexts;
using Events.Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json;
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
            return _mapper.Map<Event>(eEntity);
        }
        public async Task<Event?> GetByNameAsync(string name)
        {
            var eEntity = await _dbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Name == name);
            return _mapper.Map<Event>(eEntity);
        }
        public async Task CreateAsync(Event ev)
        {
            var eEntity = _mapper.Map<EventEntity>(ev);
            _dbContext.Events.Add(eEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event target, Event ev)
        {
            var eEntity = _mapper.Map<EventEntity>(target);
            _mapper.Map(ev, eEntity);
            _dbContext.Events.Update(eEntity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Event target)
        {
            var eEntity = _mapper.Map<EventEntity>(target);
            _dbContext.Events.Remove(eEntity);
            await _dbContext.SaveChangesAsync();
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

