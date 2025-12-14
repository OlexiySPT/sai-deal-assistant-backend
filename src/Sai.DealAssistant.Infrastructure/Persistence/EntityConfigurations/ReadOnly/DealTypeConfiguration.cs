using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities.ReadOnly;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations.ReadOnly;

public class DealTypeConfiguration : BaseReadOnlyEntityConfiguration<DealType>
{
    public override void Configure(EntityTypeBuilder<DealType> builder)
    {
        base.Configure(builder);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(150);
    }
}