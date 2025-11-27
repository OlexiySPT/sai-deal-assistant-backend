using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class SampleEmployeeConfiguration : IEntityTypeConfiguration<SampleEmployee>
{
	public void Configure(EntityTypeBuilder<SampleEmployee> builder)
	{
		builder.HasKey(c => c.Id);
		builder.Property(c => c.Id)
			.ValueGeneratedOnAdd();

		builder.Property(c => c.CustomerId)
			.IsRequired();
		builder.HasOne(c => c.Customer)
			.WithMany(x => x.Employees)
			.HasForeignKey(c => c.CustomerId)
			.HasPrincipalKey(x => x.Id);

		builder.Property(c => c.FirstName)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(c => c.LastName)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(c => c.Email)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(c => c.FullName)
                .HasMaxLength(200)
			.HasComputedColumnSql($"\"{nameof(SampleEmployee.FirstName)}\" || ' ' || \"{nameof(SampleEmployee.LastName)}\"", true);

		builder.HasIndex(c => c.FullName);
		builder.HasIndex(c => c.Email).IsUnique();
	}
}
