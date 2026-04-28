using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class DealStatusAuditConfiguration : BaseNonTrackedEntityConfiguration<DealStatusAudit>
{
    public override void Configure(EntityTypeBuilder<DealStatusAudit> builder)
    {
        base.Configure(builder);

        builder.Property(a => a.PreviousValue)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(a => a.ChangeDate)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        // Foreign key to Deal
        builder.HasIndex(a => a.DealId)
            .HasDatabaseName("IX_DealStatusAudits_DealId");

        builder.HasOne<Deal>()
            .WithMany()
            .HasForeignKey(a => a.DealId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
