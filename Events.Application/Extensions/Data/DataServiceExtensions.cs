using Events.Application.Contracts;
using Events.Application.Middleware;
using Events.Application.Validators.Core;
using Events.Application.Validators.DTO;
using Events.Application.Services;
using Events.Core.Abstractions;
using Events.Core.Models;
using Events.Infrastructure.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Events.Application.MappingProfiles;
namespace Events.Application.Extensions.Data
{
    public static class DataServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IValidator<(int, int)>, PageValidator>();
            services.AddScoped<IValidator<Event>, EventValidator>();
            services.AddScoped<IValidator<AddEventParticipantDTO>, AddParticipantDTOValidator>();
            services.AddScoped<IValidator<CreateEventDTO>, CreateEventDTOValidator>();
            services.AddScoped<IValidator<IFormFile>, ImageFileValidator>();
            services.AddScoped<IDataService, DataService>();
            services.ConfigureMapping();
            services.AddAutoMapper(typeof(ApplicationMappingProfile));
            var redisConnectionString = Environment.GetEnvironmentVariable("REDIS");
            services.ConfigureDataServiceRepositories(redisConnectionString);
        }
        public static void ConfigureDbContext(this IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            services.ConfigureDbContext(connectionString!);
        }
        public static void ConfigureMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
