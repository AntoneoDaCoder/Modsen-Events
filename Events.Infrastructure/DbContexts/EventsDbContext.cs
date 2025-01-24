using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Events.Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;
using Events.Infrastructure.Configurations;
namespace Events.Infrastructure.DbContexts
{
    public class EventsDbContext : IdentityDbContext<ParticipantEntity>
    {
        public DbSet<EventEntity> Events { get; set; }
        public DbSet<EventParticipantEntity> EventParticipants { get; set; }
        public EventsDbContext(DbContextOptions<EventsDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new EventEntityConf());
            modelBuilder.ApplyConfiguration(new EventParticipantConf());
            modelBuilder.ApplyConfiguration(new ParticipantEntityConf());
        }
    }
}
