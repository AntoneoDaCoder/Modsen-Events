using Events.Core.Abstractions;
using Microsoft.AspNetCore.Identity;
using Events.Infrastructure.DbEntities;
using Events.Core.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
namespace Events.Infrastructure.Repositories
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly UserManager<ParticipantEntity> _participantsManager;
        private readonly IMapper _mapper;
        public ParticipantRepository(UserManager<ParticipantEntity> participantsManager, IMapper mapper)
        {
            _participantsManager = participantsManager;
            _mapper = mapper;
        }
        public async Task<Participant?> GetByIdAsync(Guid id)
        {
            var pEntity = await _participantsManager.FindByIdAsync(id.ToString());
            if (pEntity is not null)
                return _mapper.Map<Participant>(pEntity);
            return null;
        }
        public async Task<(bool, Participant?)> CheckPasswordAsync(string email, string password)
        {
            var actualEntity = await _participantsManager.FindByEmailAsync(email);
            return (await _participantsManager.CheckPasswordAsync(actualEntity, password), _mapper.Map<Participant>(actualEntity));
        }
        public async Task<Participant?> GetByEmailAsync(string email)
        {
            var pEntity = await _participantsManager.FindByEmailAsync(email);
            if (pEntity is not null)
                return _mapper.Map<Participant>(pEntity);
            return null;
        }
        public async Task<List<Participant>> GetPagedAsync(int index, int pageSize)
        {
            var participantEntities = await _participantsManager.Users.AsNoTracking().Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
            return _mapper.Map<List<Participant>>(participantEntities);
        }
        public async Task<(bool, IEnumerable<string>)> CreateAsync(Participant participant, string password)
        {
            var participantEntity = _mapper.Map<ParticipantEntity>(participant);
            var result = await _participantsManager.CreateAsync(participantEntity, password);
            return (result.Succeeded, result.Errors.Select(e => e.Description));
        }
        public async Task<(bool, IEnumerable<string>)> UpdateAsync(Participant participant)
        {
            var pEntity = await _participantsManager.FindByIdAsync(participant.Id.ToString());
            if (pEntity is not null)
                _mapper.Map(participant, pEntity);
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
