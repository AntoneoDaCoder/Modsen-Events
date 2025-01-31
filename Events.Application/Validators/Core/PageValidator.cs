using FluentValidation;

namespace Events.Application.Validators.Core
{
    public class PageValidator:AbstractValidator<(int,int)>
    {
        public PageValidator()
        {
            RuleFor(v => v.Item1).GreaterThanOrEqualTo(1).WithMessage("Page number should be 1 or greater.");
            RuleFor(v => v.Item2).GreaterThanOrEqualTo(1).WithMessage("Page size should be 1 or greater.");
        }
    }
}
