using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class EventNoteConfiguration : BaseEntityConfiguration<EventNote>
{
    public override void Configure(EntityTypeBuilder<EventNote> builder)
    {
        builder.Property(i => i.Text)
            .HasColumnType("text");

        builder.HasOne(i => i.Event)
            .WithMany(e => e.Notes)
            .HasForeignKey(i => i.EventId);
    }
}
