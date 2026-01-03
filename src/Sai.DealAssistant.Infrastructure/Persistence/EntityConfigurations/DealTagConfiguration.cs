using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class DealTagConfiguration : BaseEntityConfiguration<DealTag>
{
    public override void Configure(EntityTypeBuilder<DealTag> builder) {
        base.Configure(builder);

        builder.Property(i => i.Tag)
            .HasMaxLength(100);

        builder.HasOne(i => i.Deal)
            .WithMany(e => e.Tags)
            .HasForeignKey(i => i.DealId);

        builder.HasIndex(b => b.Tag)
            .HasDatabaseName("IX_DealTagss_Lower90_Tag")
            .HasAnnotation("Npgsql:IndexExpression", "lower(left(\"Tag\", 90))");
    }
}
