using Events.Application.Contracts;
using FluentValidation;
namespace Events.Application.Validators.DTO
{
    public class RegisterDTOValidator : AbstractValidator<RegisterParticipantDTO>
    {
        public RegisterDTOValidator()
        {
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Email).NotEmpty();
            RuleFor(p => p.Surname).NotEmpty();
            RuleFor(p => p.BirthDate).NotEmpty();
        }
    }
}
