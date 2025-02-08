using AutoMapper;
using Events.Core.Abstractions;
using Events.Core.Contracts;
using Events.Infrastructure.DbContexts;
using Events.Infrastructure.DbEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Events.Infrastructure.Repositories
{
    public class EventParticipantRepository : IEventParticipantRepository
    {
        private readonly EventsDbContext _dbContext;
        private readonly IMapper _mapper;
        public EventParticipantRepository(EventsDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task RegisterParticipantAsync(Guid eventId, string participantId)
        {
            var eventParticipantEntity = new EventParticipantEntity()
            {
                EventId = eventId,
                ParticipantId = participantId,
                RegisterDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            _dbContext.EventParticipants.Add(eventParticipantEntity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<ParticipantWithDateDTO>> GetPagedParticipantsAsync(Guid eventId, int index, int pageSize)
        {
            var eventParticipants = await _dbContext.EventParticipants
                .AsNoTracking()
                .Where(ep => ep.EventId == eventId)
                .Include(ep => ep.Participant)
                .Skip((index - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return _mapper.Map<List<ParticipantWithDateDTO>>(eventParticipants);
        }
        public async Task<ParticipantWithDateDTO?> GetEventParticipantByIdAsync(Guid eventId, string participantId)
        {
            var eventParticipant = await _dbContext.EventParticipants
                .AsNoTracking()
                .Include(ep => ep.Participant)
                .FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.ParticipantId == participantId);
            return _mapper.Map<ParticipantWithDateDTO>(eventParticipant);
        }
        public async Task UnregisterParticipantAsync(Guid eventId, string participantId, DateOnly regDate)
        {
            var eventParticipantEntity = new EventParticipantEntity()
            {
                EventId = eventId,
                ParticipantId = participantId,
                RegisterDate = regDate
            };
            _dbContext.EventParticipants.Remove(eventParticipantEntity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
