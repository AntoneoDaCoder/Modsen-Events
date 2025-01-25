using Events.Core.Abstractions;
using Microsoft.AspNetCore.Identity;
using Events.Infrastructure.DbEntities;
using Events.Core.Models;
using Microsoft.EntityFrameworkCore;
namespace Events.Infrastructure.Repositories
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly UserManager<ParticipantEntity> _participantsManager;
        public ParticipantRepository(UserManager<ParticipantEntity> participantsManager)
        {
            _participantsManager = participantsManager;
        }
        public async Task<Participant?> GetByIdAsync(Guid id)
        {
            var pEntity = await _participantsManager.FindByIdAsync(id.ToString()!);
            if (pEntity is not null)
                return Participant.CreateParticipant(Guid.Parse(pEntity.Id), pEntity.UserName!, pEntity.Surname, pEntity.BirthDate, pEntity.Email!);
            return null;
        }
        public async Task<List<Participant>> GetPagedAsync(int index, int pageSize)
        {
            var participantEntities = await _participantsManager.Users.AsNoTracking().Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
            return participantEntities.Select(e => Participant.CreateParticipant(Guid.Parse(e.Id), e.UserName!, e.Surname, e.BirthDate, e.Email!)).ToList();
        }
        public async Task<(bool, IEnumerable<string>)> CreateAsync(Participant participant, string password)
        {
            var participantEntity = new ParticipantEntity()
            {
                //auto mapper here
                Email = participant.Email,
                UserName = participant.Name,
                Surname = participant.Surname,
                BirthDate = participant.BirthDate
            };
            var result = await _participantsManager.CreateAsync(participantEntity, password);
            return (result.Succeeded, result.Errors.Select(e => e.Description));
        }
        public async Task<(bool, IEnumerable<string>)> UpdateAsync(Guid id, Participant participant)
        {
            var pEntity = await _participantsManager.FindByIdAsync(id.ToString());
            if (pEntity is not null)
            {
                //auto mapper here
                pEntity.UserName = participant.Name;
                pEntity.Surname = participant.Surname;
                pEntity.Email = participant.Email;
                pEntity.BirthDate = participant.BirthDate;
            }
            var result = await _participantsManager.UpdateAsync(pEntity!);
            return (result.Succeeded, result.Errors.Select(e => e.Description));
        }
        public async Task<(bool, IEnumerable<string>)> DeleteAsync(Guid id)
        {
            var pEntity = await _participantsManager.FindByIdAsync(id.ToString());
            var result = await _participantsManager.DeleteAsync(pEntity!);
            return (result.Succeeded, result.Errors.Select(e => e.Description));
        }
    }
}
