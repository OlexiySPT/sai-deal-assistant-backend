using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

public class AiRequestConfiguration : IEntityTypeConfiguration<AiRequest>
{
    public void Configure(EntityTypeBuilder<AiRequest> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Type)
            .IsRequired();

        builder.Property(e => e.Model)
            .IsRequired();

        builder.Property(e => e.Prompt)
            .HasColumnType("text");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamptz");
    }
}
