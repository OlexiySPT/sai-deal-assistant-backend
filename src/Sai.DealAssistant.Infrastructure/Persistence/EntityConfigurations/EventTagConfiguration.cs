using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class EventTagConfiguration : BaseEntityConfiguration<EventTag>
{
    public void Configure(EntityTypeBuilder<EventTag> builder) {
        base.Configure(builder);

        builder.Property(i => i.Tag)
            .HasMaxLength(4000);

        builder.HasOne(i => i.Event)
            .WithMany(e => e.Tags)
            .HasForeignKey(i => i.EventId);
    }
}
