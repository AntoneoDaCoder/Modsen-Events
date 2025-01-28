
using FluentValidation;
using Events.Core.Models;
namespace Events.Application.Validators.Core
{
    public class EventValidator:AbstractValidator<Event>
    {
        public EventValidator()
        {
            RuleFor(e => e)
                .Must(e => IsDateTimeInFuture(e.Date, e.Time))
                .WithMessage("Event date and time cannot be in the past.");
            RuleFor(e => e.MaxParticipants).InclusiveBetween((uint)1, (uint)10).WithMessage("The maximum number of participants must be in the range from 1 to 10 (inclusive).");
        }

        private bool IsDateTimeInFuture(DateOnly date, TimeOnly time)
        {
            var eventDateTime = date.ToDateTime(time); 
            return eventDateTime >= DateTime.Now; 
        }
    }
}
