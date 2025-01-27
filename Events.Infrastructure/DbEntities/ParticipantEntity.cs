using Microsoft.AspNetCore.Identity;
namespace Events.Infrastructure.DbEntities
{
    public class ParticipantEntity : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; } = DateOnly.MinValue;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; } = DateTime.MinValue;
        public ICollection<EventParticipantEntity> EventParticipants { get; set; } = new List<EventParticipantEntity>();
    }
}
