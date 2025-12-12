using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class ContragentContactRepConfiguration : BaseEntityConfiguration<ContragentContactRep>
{
    public override void Configure(EntityTypeBuilder<ContragentContactRep> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Position)
            .HasColumnType("varchar")
            .HasMaxLength(100);

        builder.Property(c => c.Phone)
            .HasColumnType("varchar")
            .HasMaxLength(50);

        builder.Property(c => c.Email)
            .HasColumnType("varchar")
            .HasMaxLength(150);

        builder.Property(c => c.Description)
            .HasColumnType("text");

        builder.HasOne(c => c.Contragent)
            .WithMany(co => co.ContactReps)
            .HasForeignKey(c => c.ContragentId);
    }
}
