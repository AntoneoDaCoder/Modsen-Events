namespace Events.Core.Models
{
    public class Participant
    {
        public Guid Id { get; } = Guid.Empty;
        public string Name { get; } = string.Empty;
        public string Surname { get; } = string.Empty;
        public DateOnly BirthDate { get; } = DateOnly.MinValue;
        public string Email { get; } = string.Empty;
        private Participant(Guid id, string name, string surname, DateOnly birthDate,  string email)
        {
            Id = id;
            Name = name;
            Surname = surname;
            BirthDate = birthDate;
            Email = email;
        }
        static public Participant CreateParticipant(Guid id, string name, string surname, DateOnly birthDate,  string email)
        {
            return new Participant(id, name, surname, birthDate, email);
        }

    }
}
