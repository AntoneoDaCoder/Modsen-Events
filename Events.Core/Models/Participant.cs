namespace Events.Core.Models
{
    public class Participant
    {
        public Guid Id { get; } = Guid.Empty;
        public string Name { get; } = string.Empty;
        public string Surname { get; } = string.Empty;
        public DateOnly BirthDate { get; } = DateOnly.MinValue;
        public string Email { get; } = string.Empty;
        public string RefreshToken { get; private set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; private set; } = DateTime.MinValue;
        private Participant(Guid id, string name, string surname, DateOnly birthDate, string email, string refreshToken, DateTime expirationTime)
        {
            Id = id;
            Name = name;
            Surname = surname;
            BirthDate = birthDate;
            Email = email;
            RefreshTokenExpiryTime = expirationTime;
            RefreshToken = refreshToken;
        }
        private Participant(Guid id, string name, string surname, DateOnly birthDate, string email)
        {
            Id = id;
            Name = name;
            Surname = surname;
            BirthDate = birthDate;
            Email = email;
        }
        static public Participant CreateParticipant(Guid id, string name, string surname, DateOnly birthDate, string email, string refreshToken, DateTime expirationTime)
        {
            return new Participant(id, name, surname, birthDate, email, refreshToken, expirationTime);
        }
        static public Participant CreateParticipant(Guid id, string name, string surname, DateOnly birthDate, string email)
        {
            return new Participant(id, name, surname, birthDate, email);
        }
        public void SetRefreshToken(string token, DateTime expirationTime)
        {
            RefreshToken = token;
            RefreshTokenExpiryTime = expirationTime;
        }

    }
}
