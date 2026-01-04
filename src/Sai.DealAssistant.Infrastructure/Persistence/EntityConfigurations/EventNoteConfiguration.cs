using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class EventNoteConfiguration : BaseNonTrackedEntityConfiguration<EventNote>
{
    public override void Configure(EntityTypeBuilder<EventNote> builder)
    {
        builder.Property(i => i.Text)
            .HasColumnType("text");

        builder.HasOne(i => i.Event)
            .WithMany(e => e.Notes)
            .HasForeignKey(i => i.EventId);

        // Prevent reassigning EventNote to another Event after insert.
        // This will cause EF Core to ignore if code attempts to change EventId and call SaveChanges.
        var eventIdProp = builder.Property(e => e.EventId);
        eventIdProp.ValueGeneratedNever(); // explicit: not DB-generated
        eventIdProp.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
