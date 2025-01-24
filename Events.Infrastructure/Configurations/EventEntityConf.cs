using Microsoft.EntityFrameworkCore;
using Events.Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Events.Infrastructure.Configurations
{
    public class EventEntityConf : IEntityTypeConfiguration<EventEntity>
    {
        public void Configure(EntityTypeBuilder<EventEntity> builder)
        {
            builder.HasKey(ev => ev.Id);
            builder.Property(ev => ev.Name).IsRequired();
            builder.Property(ev => ev.Description).IsRequired();
            builder.Property(ev => ev.Date).IsRequired();
            builder.Property(ev => ev.Time).IsRequired();
            builder.Property(ev => ev.Location).IsRequired();
            builder.Property(ev => ev.Category).IsRequired();
            builder.Property(ev => ev.MaxParticipants).IsRequired();
            builder.Property(ev => ev.ImagePath).IsRequired();
            builder.HasIndex(ev => new { ev.Date, ev.Location, ev.Category });
        }
    }
}
