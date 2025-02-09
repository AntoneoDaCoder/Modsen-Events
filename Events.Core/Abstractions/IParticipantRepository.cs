﻿using Events.Core.Models;
namespace Events.Core.Abstractions
{
    public interface IParticipantRepository
    {
        Task<Participant?> GetByIdAsync(Guid id);
        Task<Participant?> GetByEmailAsync(string email);
        Task<List<Participant>> GetPagedAsync(int index, int pageSize);
        Task<(bool,Participant?)> CheckPasswordAsync(string email, string password);
        Task<(string, IEnumerable<string>)> CreateAsync(Participant participant, string password);
        Task<(bool, IEnumerable<string>)> UpdateAsync(Participant participant);
        Task<(bool, IEnumerable<string>)> DeleteAsync(Guid id);
    }
}
