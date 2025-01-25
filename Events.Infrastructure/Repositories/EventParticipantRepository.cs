using Events.Core.Abstractions;
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
        public EventParticipantRepository(EventsDbContext dbContext, UserManager<ParticipantEntity> manager)
        {
            _dbContext = dbContext;
            _participantManager = manager;
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
    }
}
