using Events.Application.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace Events.Application.Authorization.Handlers
{
    public class MinimumAgeHandler : AuthorizationHandler<AgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AgeRequirement requirement)
        {
            var dateOfBirthClaim = context.User.FindFirst(
            c => c.Type == ClaimTypes.DateOfBirth && c.Issuer == "auth-service");

            if (dateOfBirthClaim is null)
            {
                return Task.CompletedTask;
            }
            var dateOfBirth = DateOnly.Parse(dateOfBirthClaim.Value);
            int calculatedAge = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth > DateOnly.FromDateTime(DateTime.Today.AddYears(-calculatedAge)))
            {
                calculatedAge--;
            }

            if (calculatedAge >= requirement.MinimumAge)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
