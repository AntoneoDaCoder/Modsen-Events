using Microsoft.EntityFrameworkCore;
using Events.Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Events.Infrastructure.Configurations
{
    public class ParticipantEntityConf : IEntityTypeConfiguration<ParticipantEntity>
    {
        public void Configure(EntityTypeBuilder<ParticipantEntity> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Surname).IsRequired();
            builder.Property(p => p.BirthDate).IsRequired();
        }
    }
}
