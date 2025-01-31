using Events.Application.Contracts;
using FluentValidation;

namespace Events.Application.Validators.DTO
{
    public class CreateEventDTOValidator:AbstractValidator<CreateEventDTO>
    {
        public CreateEventDTOValidator()
        {
            RuleFor(e => e.Date).NotEmpty();
            RuleFor(e => e.Category).NotEmpty();
            RuleFor(e => e.MaxParticipants).NotEmpty();
            RuleFor(e => e.Description).NotEmpty();
            RuleFor(e => e.Location).NotEmpty();
            RuleFor(e => e.Time).NotEmpty();
            RuleFor(e => e.Name).NotEmpty();
        }
    }
}
