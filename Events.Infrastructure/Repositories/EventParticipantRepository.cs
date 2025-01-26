using AutoMapper;
using Events.Core.Abstractions;
using Events.Core.Models;
using Events.Infrastructure.DbContexts;
using Events.Infrastructure.DbEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Events.Infrastructure.Repositories
{
    public class EventParticipantRepository : IEventParticipantRepository
    {
        private readonly EventsDbContext _dbContext;
        private readonly UserManager<ParticipantEntity> _participantManager;
        private readonly IMapper _mapper;
        public EventParticipantRepository(EventsDbContext dbContext, UserManager<ParticipantEntity> manager, IMapper mapper)
        {
            _dbContext = dbContext;
            _participantManager = manager;
            _mapper = mapper;
        }
        public async Task<(bool, IEnumerable<string>)> RegisterParticipantAsync(Guid eventId, Guid participantId)
        {
            var errors = new List<string>();
            var eEntity = await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            var pEntity = await _participantManager.FindByIdAsync(participantId.ToString());

            if (eEntity is null)
                errors.Add("Event not found.");
            if (pEntity is null)
                errors.Add("Participant not found.");

            if (errors.Count != 0)
                return (false, errors);

            var eventParticipantEntity = new EventParticipantEntity()
            {
                EventId = eventId,
                ParticipantId = participantId,
                RegisterDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            try
            {
                _dbContext.EventParticipants.Add(eventParticipantEntity);
                await _dbContext.SaveChangesAsync();
                return (true, Enumerable.Empty<string>());
            }
            catch (DbUpdateException ex)
            {
                errors.Add(ex.Message);
                if (ex.InnerException != null)
                {
                    errors.Add(ex.InnerException.Message);
                }
                return (false, errors);
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return (false, errors);
            }
        }
        public async Task<(List<Participant>, IEnumerable<string>)> GetPagedParticipantsAsync(Guid eventId, int index, int pageSize)
        {
            var eventParticipants = await _dbContext.EventParticipants
                .AsNoTracking()
                .Where(ep => ep.EventId == eventId)
                .Include(ep => ep.Participant)
                .Skip((index - 1) * pageSize)
                .Take(pageSize)
                .Select(ep => ep.Participant)
                .ToListAsync();
            if (eventParticipants.Count == 0)
                return (new List<Participant>(), new List<string> { "Event not found or has no participants." });
            var participants = _mapper.Map<List<Participant>>(eventParticipants);
            return (participants, Enumerable.Empty<string>());
        }
        public async Task<(bool, IEnumerable<string>)> UnregisterParticipantAsync(Guid eventId, Guid participantId)
        {
            var eventParticipantEntity = await _dbContext.EventParticipants.FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.ParticipantId == participantId);
            if (eventParticipantEntity is not null)
            {
                try
                {
                    _dbContext.EventParticipants.Remove(eventParticipantEntity);
                    await _dbContext.SaveChangesAsync();
                    return (true, Enumerable.Empty<string>());
                }
                catch (DbUpdateException ex)
                {
                    var errors = new List<string>() { ex.Message };
                    if (ex.InnerException != null)
                    {
                        errors.Add(ex.InnerException.Message);
                    }
                    return (false, errors);
                }
                catch (Exception ex)
                {
                    return (false, new List<string>() { ex.Message });
                }
            }
            return (false, new List<string>() { "Event participant not found." });
        }
    }
}
