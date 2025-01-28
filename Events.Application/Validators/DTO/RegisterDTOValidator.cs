using Events.Application.Contracts;
using FluentValidation;
namespace Events.Application.Validators.DTO
{
    public class RegisterDTOValidator : AbstractValidator<RegisterParticipantDTO>
    {
        public RegisterDTOValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .Matches("^[a-zA-Z]+$")
                .WithMessage("The field must contain only letters.");
            RuleFor(p => p.Email).NotEmpty();
            RuleFor(p => p.Surname)
                .NotEmpty()
                .Matches("^[a-zA-Z]+$")
                .WithMessage("The field must contain only letters.");
            RuleFor(p => p.BirthDate).NotEmpty();
        }
    }
}
