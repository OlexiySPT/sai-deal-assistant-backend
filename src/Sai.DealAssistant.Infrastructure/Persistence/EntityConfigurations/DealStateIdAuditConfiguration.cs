using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class DealStateIdAuditConfiguration : BaseNonTrackedEntityConfiguration<DealStateIdAudit>
{
    public override void Configure(EntityTypeBuilder<DealStateIdAudit> builder)
    {
        base.Configure(builder);

        builder.Property(a => a.PreviousText)
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(a => a.ChangeDate)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(a => a.DealId)
            .HasDatabaseName("IX_DealStateIdAudits_DealId");

        builder.HasOne<Deal>()
            .WithMany()
            .HasForeignKey(a => a.DealId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
