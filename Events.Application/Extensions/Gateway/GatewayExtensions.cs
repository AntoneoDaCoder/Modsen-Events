using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Events.Application.Services;
using Events.Application.Authorization.Requirements;
using Events.Application.Authorization.Handlers;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Events.Application.Middleware;
namespace Events.Application.Extensions.Gateway
{
    public static class GatewayExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IRequestRouter, GatewayRequestRouter>();
            services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();
            services.AddHttpClient
            (
                "auth-service", client =>
                {
                    client.BaseAddress = new Uri("http://auth-service:8081/");
                }
            );
        }
        public static void ConfigureAuthorization(this IServiceCollection services, IConfiguration conf)
        {
            var jwtSettings = conf.GetSection("JwtSettings");
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET");


            services.AddAuthorization(options =>
            {
                options.AddPolicy("AgeOver18", policy =>
                    policy.Requirements.Add(new AgeRequirement(18)));
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings["validIssuer"],
                    ValidAudience = jwtSettings["validAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
                };
            });
        }
        public static void ConfigureMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
