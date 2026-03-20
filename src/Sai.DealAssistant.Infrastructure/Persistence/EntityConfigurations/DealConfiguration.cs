using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class DealConfiguration : BaseEntityConfiguration<Deal>
{
    public override void Configure(EntityTypeBuilder<Deal> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.Name)
            .HasColumnType("varchar")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Url)
            .HasColumnType("varchar")
            .HasMaxLength(4095);

        // Converted Description to citext (case-insensitive text) per request
        builder.Property(c => c.Description)
            .HasColumnType("citext");

        builder.Property(c => c.AiSearchInfo)
            .HasColumnType("varchar")
            .HasMaxLength(4095);

        builder.Property(c => c.AiBriefDescription)
            .HasColumnType("text");

        builder.Property(c => c.Industry)
            .HasColumnType("varchar")
            .HasMaxLength(100);

        builder.Property(c => c.Status)
            .HasColumnType("varchar")
            .HasMaxLength(50);

        // New fields
        builder.Property(c => c.ProposalAmount)
            .HasColumnType("numeric(18,2)");

        builder.Property(c => c.MinClientAmount)
            .HasColumnType("numeric(18,2)");

        builder.Property(c => c.MaxClientAmount)
            .HasColumnType("numeric(18,2)");

        builder.Property(c => c.CurrencyCode)
            .HasColumnType("varchar")
            .HasMaxLength(10);

        builder.Property(c => c.ExchangeRateToEur)
            .HasColumnType("numeric(18,6)");

        builder.Property(c => c.AmountTypeId)
            .IsRequired(false);

        builder.HasOne(c => c.AmountType)
            .WithMany(a => a.Deals)
            .HasForeignKey(c => c.AmountTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // New Company field
        builder.Property(c => c.Company)
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .IsRequired();

        // Indexes
        builder.HasIndex(b => b.Name)
            .HasDatabaseName("IX_Deals_Lower90_Name")
            .HasAnnotation("Npgsql:IndexExpression", "lower(left(\"Name\", 90))");

        builder.HasIndex(b => b.Industry)
            .HasDatabaseName("IX_Deals_Lower90_Industry")
            .HasAnnotation("Npgsql:IndexExpression", "lower(left(\"Industry\", 90))");
    }
}
