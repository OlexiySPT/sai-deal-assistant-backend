using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class ContragentConfiguration : BaseEntityConfiguration<Contragent>
{
    public override void Configure(EntityTypeBuilder<Contragent> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.Name)
            .HasColumnType("varchar")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Url)
            .HasColumnType("varchar")
            .HasMaxLength(4095);

        builder.Property(c => c.Notes)
            .HasColumnType("text");

        builder.Property(c => c.AiSearchInfo)
            .HasColumnType("varchar")
            .HasMaxLength(4095);

        builder.Property(c => c.AiBriefDescription)
            .HasColumnType("text");

        builder.Property(c => c.Industry)
            .HasColumnType("varchar")
            .HasMaxLength(100);

        builder.Property(c => c.Address)
            .HasMaxLength(500);

        builder.Property(c => c.Status)
            .HasColumnType("varchar")
            .HasMaxLength(50);
    }
}
