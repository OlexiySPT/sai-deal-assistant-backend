using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

public class AiResultConfiguration : IEntityTypeConfiguration<AiResult>
{
    public void Configure(EntityTypeBuilder<AiResult> builder)
    {
        // Id configuration
        builder.HasKey(c => c.RequestId);
        builder.Property(e => e.RequestId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamptz");

        builder.Property(e => e.Result)
            .HasColumnType("text");

        builder.Property(e => e.DurationSeconds)
            .IsRequired();

        builder.Property(e => e.Success)
            .IsRequired();
        builder.HasOne<AiRequest>()
            .WithMany()
            .HasForeignKey(e => e.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}