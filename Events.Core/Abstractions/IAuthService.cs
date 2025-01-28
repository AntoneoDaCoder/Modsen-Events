using Events.Core.Models;
namespace Events.Core.Abstractions
{
    public interface IAuthService
    {
        Task<(string, string)> CreateToken(Participant p, bool refresh);
        Task<(string, string)> UpdateToken(string access, string refresh);
        Task<(bool, Participant?)> ValidateParticipantAsync(string email, string password);
        Task RegisterParticipantAsync(Participant p, string password);
    }
}
