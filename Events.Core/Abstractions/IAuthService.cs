using Events.Core.Models;
namespace Events.Core.Abstractions
{
    public interface IAuthService
    {
        Task<(string, string)> CreateToken(bool populate);
        Task<(string, string)> RefreshToken(string accessToken, string refreshToken);
        Task<(bool, Participant)> ValidateParticipantAsync(Participant p, string password);
        Task<(bool, IEnumerable<string>)> RegisterParticipantAsync(Participant p, string password);
    }
}
