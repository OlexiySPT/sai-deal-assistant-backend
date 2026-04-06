using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class FirmConfiguration : BaseEntityConfiguration<Firm>
{
    public override void Configure(EntityTypeBuilder<Firm> builder)
    {
        base.Configure(builder);

        builder.Property(f => f.Name)
            .HasColumnType("varchar")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(f => f.Name);

        builder.Property(f => f.Country)
            .HasColumnType("varchar")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(f => f.Description)
            .HasColumnType("text");

        builder.HasMany(f => f.Deals)
            .WithOne(d => d.Firm)
            .HasForeignKey(d => d.FirmId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}