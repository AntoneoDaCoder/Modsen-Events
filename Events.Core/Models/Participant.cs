namespace Events.Core.Models
{
    public class Participant
    {
        public Guid Id { get; }
        public string Name { get; } = string.Empty;
        public string Surname { get; } = string.Empty;
        public DateOnly BirthDate { get; } = DateOnly.MinValue;
        public DateOnly EventRegistrationDate { get; } = DateOnly.MinValue;
        public string Email { get; } = string.Empty;
        private Participant(Guid id, string name, string surname, DateOnly birthDate, DateOnly eventRegistrationDate, string email)
        {
            Id = id;
            Name = name;
            Surname = surname;
            BirthDate = birthDate;
            EventRegistrationDate = eventRegistrationDate;
        }
        static public Participant CreateParticipant(Guid id, string name, string surname, DateOnly birthDate, DateOnly eventRegistrationDate, string email)
        {
            return new Participant(id, name, surname, birthDate, eventRegistrationDate, email);
        }
    }
}
