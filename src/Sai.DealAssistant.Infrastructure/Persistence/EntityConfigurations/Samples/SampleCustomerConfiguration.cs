using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities.Samples;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations.Samples;

public class SampleCustomerConfiguration : BaseEntityConfiguration<SampleCustomer>
{
	public override void Configure(EntityTypeBuilder<SampleCustomer> builder)
	{
		base.Configure(builder);

        builder.Property(c => c.Code)
			.IsRequired()
			.HasColumnType("varchar")
			.HasMaxLength(50);
		builder.HasIndex(c => c.Code).IsUnique();

		builder.Property(c => c.Name)
			.IsRequired()
			.HasMaxLength(255);

		builder.Property(c => c.Phone)
			.HasColumnType("varchar")
			.HasMaxLength(50);
		builder.Property(c => c.Email)
			.HasColumnType("varchar")
			.HasMaxLength(200);


        builder.Property(c => c.TaxNumber)
            .HasColumnType("varchar")
            .HasMaxLength(50);

        builder.Property(c => c.DateRegistered)
            .HasColumnType("date");
    }
}
