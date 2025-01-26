using FluentValidation;
using Events.Core.Models;
namespace Events.Application.Validators.Core
{
    public class ParticipantValidator : AbstractValidator<Participant>
    {

        public ParticipantValidator()
        {
            Func<DateOnly, bool> isOlderThan16 = birthDate =>
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var age = today.Year - birthDate.Year;
                if (birthDate > today.AddYears(-age))
                {
                    age--;
                }

                return age >= 16;
            };

            RuleFor(x => x.BirthDate)
                .Must(isOlderThan16)
                .WithMessage("You must be 16 or older to register.");
        }

    }
}
