using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class ContactPersonConfiguration : BaseEntityConfiguration<ContactPerson>
{
    public override void Configure(EntityTypeBuilder<ContactPerson> builder)
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

        builder.HasOne(c => c.Firm)
            .WithMany(f => f.ContactPersons)
            .HasForeignKey(c => c.FirmId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevent reassigning ContactPerson to another Firm after insert.
        var firmIdProp = builder.Property(e => e.FirmId);
        firmIdProp.ValueGeneratedNever(); // explicit: not DB-generated
        firmIdProp.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
