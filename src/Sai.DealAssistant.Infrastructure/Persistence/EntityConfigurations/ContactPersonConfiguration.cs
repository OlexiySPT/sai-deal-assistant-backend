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

        builder.HasOne(c => c.Deal)
            .WithMany(co => co.ContactPersons)
            .HasForeignKey(c => c.DealId);

        // Prevent reassigning Event to another Deal after insert.
        // This will cause EF Core to ignore if code attempts to change DealId and call SaveChanges.
        var dealIdProp = builder.Property(e => e.DealId);
        dealIdProp.ValueGeneratedNever(); // explicit: not DB-generated
        dealIdProp.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
