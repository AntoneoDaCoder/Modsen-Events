using Events.Core.Models;
namespace Events.Core.Abstractions
{
    public interface IAuthService
    {
        Task<(string, string)> CreateToken(Participant p, bool refresh);
        Task<(string, string)> UpdateToken(string access, string refresh);
        Task<(string, string)> RefreshToken(string accessToken, string refreshToken);
        Task<(bool, Participant?)> ValidateParticipantAsync(Participant p, string password);
        Task<(bool, IEnumerable<string>)> RegisterParticipantAsync(Participant p, string password);
    }
}
