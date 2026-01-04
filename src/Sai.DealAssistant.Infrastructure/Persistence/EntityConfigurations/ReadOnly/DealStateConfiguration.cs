using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations.ReadOnly;

public class DealStateConfiguration : BaseNonTrackedEntityConfiguration<DealState>
{
    public override void Configure(EntityTypeBuilder<DealState> builder)
    {
        base.Configure(builder);

        builder.Property(d => d.State)
            .IsRequired()
            .HasMaxLength(50);

        // one-to-many: DealState (principal) -> Deal (dependent)
        builder.HasMany(d => d.Deals)
            .WithOne(deal => deal.State)
            .HasForeignKey(deal => deal.StateId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}