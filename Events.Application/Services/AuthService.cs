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
namespace Events.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IParticipantRepository _participantRepository;
        private readonly IConfiguration _configuration;
        private readonly IValidator<Participant> _validator;
        public AuthService(IParticipantRepository participantRepository, IConfiguration conf, IValidator<Participant> validator)
        {
            _participantRepository = participantRepository;
            _configuration = conf;
            _validator = validator;
        }
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        private static List<Claim> GetClaims(Participant p)
        {
            return new List<Claim> { new Claim(ClaimTypes.UserData, p.Id.ToString()), new Claim(ClaimTypes.DateOfBirth, p.BirthDate.ToString()) };
        }
        private static SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!)),
                ValidateLifetime = false,
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;


            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out
        securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null ||
        !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;


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
        public async Task<(string, string)> CreateToken(Participant p, bool refresh)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = GetClaims(p);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            var refreshToken = string.Empty;
            if (refresh)
            {
                refreshToken = GenerateRefreshToken();
                var expirationTime = DateTime.Now.AddDays(7);
                p.SetRefreshToken(refreshToken, expirationTime);
                var (res, errors) = await _participantRepository.UpdateAsync(p);
                if (!res && errors.Any())
                    throw EventsException.RaiseException<ServiceException>("Auth service failed to update participant's refresh token [internal error]", errors);
            }
            return (new JwtSecurityTokenHandler().WriteToken(tokenOptions), refreshToken);
        }
        public async Task<(string, string)> UpdateToken(string access, string refresh)
        {
            var principal = GetPrincipalFromExpiredToken(access);
            var p = await _participantRepository.GetByIdAsync(Guid.Parse(principal.Claims.First(x => x.Type == ClaimTypes.UserData).Value));

            if (p is null)
                throw EventsException.RaiseException<ServiceException>("Auth service failed to refresh token [internal error]", ["participant doesn't exist"]);

            if (p.RefreshToken != refresh || p.RefreshTokenExpiryTime <= DateTime.Now)
                throw EventsException.RaiseException<ServiceException>("Auth service failed to refresh token [internal error]", ["invalid tokens"]);

            var (newAccessToken, refreshToken) = await CreateToken(p, refresh: false);
            return (newAccessToken, p.RefreshToken);
        }
        public async Task<Participant> ValidateParticipantAsync(string email, string password)
        {
            var (success, participant) = await _participantRepository.CheckPasswordAsync(email, password);
            if(!success && participant is null)
                throw EventsException.RaiseException<ServiceException>("Auth service failed to verify credentials [internal error]", ["incorrect password or participant does not exist"]);
            return participant!;
        }
        public async Task RegisterParticipantAsync(Participant p, string password)
        {
            var validationResult = await _validator.ValidateAsync(p);
            if (validationResult.IsValid)
            {
                var (res, errors) = await _participantRepository.CreateAsync(p, password);
                if (!res && errors.Any())
                    throw EventsException.RaiseException<ServiceException>("Auth service failed to add participant [internal error]", errors);
            }
            else
                throw EventsException.RaiseException<IncorrectDataException>("Auth service failed to add participant [incorrect data]", validationResult.Errors.Select(e => e.ErrorMessage));
        }
    }
}
