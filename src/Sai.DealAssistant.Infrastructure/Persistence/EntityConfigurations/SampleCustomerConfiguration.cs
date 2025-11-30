using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class SampleCustomerConfiguration : IEntityTypeConfiguration<SampleCustomer>
{
	public void Configure(EntityTypeBuilder<SampleCustomer> builder)
	{
		builder.HasKey(c => c.Id);
		builder.Property(c => c.Id)
			.ValueGeneratedOnAdd();

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
