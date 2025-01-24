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
        public async Task<List<Participant>> GetAsync(Guid? id)
        {
            if (id is null)
            {
                var participantEntities = await _participantsManager.Users.AsNoTracking().ToListAsync();
                return participantEntities
                    .Select(p => Participant.CreateParticipant(Guid.Parse(p.Id), p.UserName!, p.Surname, p.BirthDate, p.Email!)).ToList();
            }
            var p = await _participantsManager.FindByIdAsync(id.ToString()!);
            if (p is not null)
                return new List<Participant> { Participant.CreateParticipant(Guid.Parse(p.Id), p.UserName!, p.Surname, p.BirthDate, p.Email!) };
            return new();
        }
        public async Task<(bool, IEnumerable<string>)> CreateAsync(Participant participant, string password)
        {
            var participantEntity = new ParticipantEntity()
            {
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
