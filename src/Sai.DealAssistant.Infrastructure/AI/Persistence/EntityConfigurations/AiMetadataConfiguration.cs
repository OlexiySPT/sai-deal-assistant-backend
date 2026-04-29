using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

public class AiMetadataConfiguration : IEntityTypeConfiguration<AiMetadata>
{
    public void Configure(EntityTypeBuilder<AiMetadata> builder)
    {
        builder.ToTable("AiMetadata");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Type)
            .HasColumnType("varchar")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Key)
            .HasColumnType("varchar")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Version)
            .HasColumnType("varchar")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.Text)
            .HasColumnType("text")
            .IsRequired();

        builder.HasIndex(e => new {e.Type, e.Key, e.Version })
            .IsUnique();

        // Ensure version contains only numbers and dots (e.g. 1, 1.0, 2.1.3)
        // builder.HasCheckConstraint("CK_AiPrompts_Version_Format", "\"Version\" ~ '^[0-9]+(\\.[0-9]+)*$'");
    }
}
