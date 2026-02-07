using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations.ReadOnly;

public class AmountTypeConfiguration : BaseNonTrackedEntityConfiguration<AmountType>
{
    public override void Configure(EntityTypeBuilder<AmountType> builder)
    {
        base.Configure(builder);

        builder.Property(a => a.Type)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasMany(a => a.Deals)
            .WithOne(d => d.AmountType)
            .HasForeignKey(d => d.AmountTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}