using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Events.Application.Validators.DTO
{
    public class ImageFileValidator : AbstractValidator<IFormFile>
    {
        const int MAX_IMG_SIZE = 3145728;
        public ImageFileValidator()
        {
            RuleFor(i => i.Length).LessThanOrEqualTo(MAX_IMG_SIZE).WithMessage("Image size must be less than or equal to 3Mb.");
            RuleFor(i => i.ContentType).Matches("image/png").WithMessage("Incorrect image format.");
        }
    }
}
