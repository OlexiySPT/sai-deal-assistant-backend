using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

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

        // Create expression indexes for lower(left(...,90)) on Name and Industry.
        // This uses the Npgsql EF Core provider index-expression annotation which the Npgsql migrations
        // codegen understands and will emit as a PostgreSQL expression index.
        builder.HasIndex(b => b.Name)
            .HasDatabaseName("IX_Deals_Lower90_Name")
            .HasAnnotation("Npgsql:IndexExpression", "lower(left(\"Name\", 90))");

        builder.HasIndex(b => b.Industry)
            .HasDatabaseName("IX_Deals_Lower90_Industry")
            .HasAnnotation("Npgsql:IndexExpression", "lower(left(\"Industry\", 90))");
    }
}
