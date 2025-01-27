using Events.Application.Contracts;
using Events.Application.Extensions.Auth;
using Events.Application.Services;
using Events.Application.Validators.Core;
using Events.Application.Validators.DTO;
using Events.Core.Abstractions;
using Events.Infrastructure.Extensions;
using Events.Core.Models;
using Events.Application.MappingProfiles;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Events.Application.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Events.Application.Extensions.Auth
{
    public static class AuthExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IValidator<LoginParticipantDTO>, LoginDTOValidator>();
            services.AddScoped<IValidator<RegisterParticipantDTO>, RegisterDTOValidator>();
            services.AddScoped<IValidator<Participant>, ParticipantValidator>();
            services.AddScoped<IAuthService, AuthService>();
            services.ConfigureMapping();
            services.ConfigureAuthRepositories();
            services.AddAutoMapper(typeof(ApplicationMappingProfile));

        }
        public static void ConfigureMiddleware(this IApplicationBuilder app)
        {
           // app.UseMiddleware<ExceptionMiddleware>();
        }
        public static void ConfigureDbContext(this IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            services.ConfigureDbContext(connectionString!);
        }
    }
}
