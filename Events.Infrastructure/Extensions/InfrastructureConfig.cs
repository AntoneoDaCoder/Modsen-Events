using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Events.Infrastructure.DbContexts;
using Events.Infrastructure.MappingProfiles;
using Events.Infrastructure.DbEntities;
namespace Events.Infrastructure.Extensions
{
    public static class InfrastructureConfig
    {
        public static void ConfigureInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddAutoMapper(cfg => { cfg.AddExpressionMapping(); }, typeof(InfrastructureMappingProfile));

            services.AddDbContext<EventsDbContext>
               (
               options => options.UseNpgsql(connectionString)
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
    }
}
