using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Events.Infrastructure.DbContexts;
class Program
{
    static void Main(string[] args)
    {
        var serviceProvider = BuildServiceProvider();
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<EventsDbContext>();

            try
            {
                Console.WriteLine("Applying migrations...");
                dbContext.Database.Migrate();
                Console.WriteLine("Migrations applied successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
            }
        }
    }

    static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddDbContext<EventsDbContext>(options =>
            options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING")));
        return services.BuildServiceProvider();
    }
}