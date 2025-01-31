using Events.Application.Contracts;
using FluentValidation;
namespace Events.Application.Validators.DTO
{
    public class AddParticipantDTOValidator:AbstractValidator<AddEventParticipantDTO>
    {
        public AddParticipantDTOValidator() 
        {
            RuleFor(p=>p.ParticipantId).NotEmpty();
        }
    }
}
