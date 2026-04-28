using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

public class AiPromptConfiguration : IEntityTypeConfiguration<AiPrompt>
{
    public void Configure(EntityTypeBuilder<AiPrompt> builder)
    {
        builder.ToTable("AiPrompts");

        builder.HasKey(e => e.Id);

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

        builder.HasIndex(e => new { e.Key, e.Version })
            .IsUnique();

        // Ensure version contains only numbers and dots (e.g. 1, 1.0, 2.1.3)
        // builder.HasCheckConstraint("CK_AiPrompts_Version_Format", "\"Version\" ~ '^[0-9]+(\\.[0-9]+)*$'");
    }
}
