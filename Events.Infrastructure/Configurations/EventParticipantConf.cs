using Microsoft.EntityFrameworkCore;
using Events.Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Events.Infrastructure.Configurations
{
    public class EventParticipantConf : IEntityTypeConfiguration<EventParticipantEntity>
    {
        public void Configure(EntityTypeBuilder<EventParticipantEntity> builder)
        {
            builder.HasKey(ep => new { ep.EventId, ep.ParticipantId });
            builder
                .HasOne(p => p.Participant)
                .WithMany(ep => ep.EventParticipants)
                .HasForeignKey(ep => ep.ParticipantId).OnDelete(DeleteBehavior.Cascade);

            builder
               .HasOne(e => e.Event)
               .WithMany(ep => ep.EventParticipants)
               .HasForeignKey(ep => ep.EventId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
