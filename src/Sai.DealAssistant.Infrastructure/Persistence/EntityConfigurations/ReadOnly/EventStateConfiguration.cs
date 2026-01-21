using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations.ReadOnly;

public class EventStateConfiguration : BaseNonTrackedEntityConfiguration<EventState>
{
    public override void Configure(EntityTypeBuilder<EventState> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.State)
            .IsRequired()
            .HasMaxLength(50);

        // one-to-many: EventState (principal) -> Event (dependent)
        builder.HasMany(e => e.Events)
            .WithOne(ev => ev.State)
            .HasForeignKey(ev => ev.StateId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
