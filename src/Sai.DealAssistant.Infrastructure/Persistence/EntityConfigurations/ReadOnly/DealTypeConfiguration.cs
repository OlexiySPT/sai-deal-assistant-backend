using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations.ReadOnly;

public class DealTypeConfiguration : BaseNonTrackedEntityConfiguration<DealType>
{
    public override void Configure(EntityTypeBuilder<DealType> builder)
    {
        base.Configure(builder);

        builder.Property(d => d.Type)
            .IsRequired()
            .HasMaxLength(150);

        // one-to-many: DealState (principal) -> Deal (dependent)
        builder.HasMany(d => d.Deals)
            .WithOne(deal => deal.Type)
            .HasForeignKey(deal => deal.TypeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}