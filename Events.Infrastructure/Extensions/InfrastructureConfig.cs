using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Events.Infrastructure.DbContexts;
using Events.Infrastructure.MappingProfiles;
using Events.Infrastructure.DbEntities;
using Events.Infrastructure.Repositories;
using Events.Core.Abstractions;
namespace Events.Infrastructure.Extensions
{
    public static class InfrastructureConfig
    {
        public static void ConfigureDbContext(this IServiceCollection services, string dbConnectionString)
        {
            services.AddDbContext<EventsDbContext>
               (
               options => options.UseNpgsql(dbConnectionString)
               );
            services.AddIdentity<ParticipantEntity, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<EventsDbContext>()
            .AddDefaultTokenProviders();
        }
        public static void ConfigureMapping(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { cfg.AddExpressionMapping(); }, typeof(InfrastructureMappingProfile));
        }
        public static void ConfigureAuthRepositories(this IServiceCollection services)
        {
            services.AddScoped<IParticipantRepository, ParticipantRepository>();
        }
        public static void ConfigureDataServiceRepositories(this IServiceCollection services, string redisConnectionString)
        {
            services.AddScoped<IEventParticipantRepository, EventParticipantRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IParticipantRepository,ParticipantRepository>();
            services.AddStackExchangeRedisCache
                (
                options =>
                {
                    options.Configuration = redisConnectionString;
                    options.InstanceName = "RedisCache_";
                }
                );
        }
    }
}
