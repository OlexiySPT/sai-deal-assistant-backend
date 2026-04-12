using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class EventConfiguration : BaseEntityConfiguration<Event>
{
    public override void Configure(EntityTypeBuilder<Event> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.Date)
            .HasColumnType("timestamptz");


        builder.Property(e => e.Topic)
            
            .HasMaxLength(200);

        builder.Property(e => e.Agenda)
            .HasColumnType("text");

        builder.Property(e => e.Result)
            .HasColumnType("text");

        // Indexes
        builder.HasIndex(e => e.Date);

        // FKs
        builder.HasOne(e => e.Deal)
            .WithMany(c => c.Events)
            .HasForeignKey(e => e.DealId);

        // Prevent reassigning Event to another Deal after insert.
        // This will cause EF Core to ignore if code attempts to change DealId and call SaveChanges.
        var dealIdProp = builder.Property(e => e.DealId);
        dealIdProp.ValueGeneratedNever(); // explicit: not DB-generated
        dealIdProp.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}