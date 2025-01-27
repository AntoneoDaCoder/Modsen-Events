using Events.Application.Contracts;
using FluentValidation;
namespace Events.Application.Validators.DTO
{
    public class LoginDTOValidator:AbstractValidator<LoginParticipantDTO>
    {
        public LoginDTOValidator()
        {
            RuleFor(d => d.Password).NotEmpty();
            RuleFor(d => d.Email).NotEmpty();
        }
    }
}
