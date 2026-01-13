using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
       base.Configure(builder);

        builder.Property(x => x.Username)
            .IsRequired()
            .HasColumnType("varchar")
            .HasMaxLength(100);

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Role)
            .IsRequired()
            .HasColumnType("varchar")
            .HasMaxLength(50);
    }
}
