using FluentValidation;
using Events.Application.Contracts;
namespace Events.Application.Validators.DTO
{
    public class RefreshDTOValidator : AbstractValidator<RefreshTokenDTO>
    {
        public RefreshDTOValidator()
        {
            RuleFor(x => x.AccessToken).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
}
