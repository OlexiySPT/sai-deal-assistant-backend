using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations.ReadOnly;

public class EventTypeConfiguration : BaseNonTrackedEntityConfiguration<EventType>
{
    public override void Configure(EntityTypeBuilder<EventType> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(150);


        // one-to-many: EventState (principal) -> Event (dependent)
        builder.HasMany(e => e.Events)
            .WithOne(ev => ev.Type)
            .HasForeignKey(ev => ev.TypeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
