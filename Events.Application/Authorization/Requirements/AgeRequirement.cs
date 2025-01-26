using Microsoft.AspNetCore.Authorization;
namespace Events.Application.Authorization.Requirements
{
    public class AgeRequirement : IAuthorizationRequirement
    {
        public int MinimumAge { get; }
        public AgeRequirement(int minimumAge) => MinimumAge = minimumAge;
    }
}
