using Events.Core.Abstractions;
using Events.Core.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using FluentValidation;
using Events.Application.Exceptions;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel;
namespace Events.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IParticipantRepository _participantRepository;
        private readonly IConfiguration _configuration;
        private readonly IValidator<Participant> _validator;
        public AuthService(IParticipantRepository participantRepository, IConfiguration conf)
        {
            _participantRepository = participantRepository;
            _configuration = conf;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        private List<Claim> GetClaims(Participant p)
        {
            return new List<Claim> { new Claim(ClaimTypes.Email, p.Email), new Claim(ClaimTypes.DateOfBirth, p.BirthDate.ToString()) };
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            return new JwtSecurityToken(
                issuer: jwtSettings["validIssuer"],
                audience: jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
                signingCredentials: signingCredentials
            );
        }
        public Task<(string, string)> CreateToken(bool populate)
        {

        }
        public async Task<(bool, Participant)> ValidateParticipantAsync(Participant p, string password)
        {

        }
        public async Task<(bool, IEnumerable<string>)> RegisterParticipantAsync(Participant p, string password)
        {
            var validationResult = await _validator.ValidateAsync(p);
            if (validationResult.IsValid)
            {
                var (success, errors) = await _participantRepository.CreateAsync(p, password);
                if (success)
                    return (success, Enumerable.Empty<string>());
                StringBuilder sb = new StringBuilder();
                foreach (var error in errors)
                    sb.Append(error + ", ");
                sb.Remove(sb.Length - 1, 1);
                throw new ServiceException("Auth service failed to add new participant: " + sb.ToString());
            }
            else
            {
                var errors = new List<string>();
                foreach (var error in validationResult.Errors)
                    errors.Add(error.PropertyName + ": " + error.ErrorMessage);
                return (false, errors);
            }
        }
    }
}
