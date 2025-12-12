using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class EventConfiguration : BaseEntityConfiguration<Event>
{
    public override void Configure(EntityTypeBuilder<Event> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.Date)
            .HasColumnType("timestamp");

        builder.Property(e => e.Status)
            .IsRequired()
            .HasColumnType("varchar")
            .HasMaxLength(50);

        builder.Property(e => e.Agenda)
            .HasColumnType("text");

        builder.Property(e => e.Result)
            .HasColumnType("text");

        builder.HasOne(e => e.Contragent)
            .WithMany(c => c.Events)
            .HasForeignKey(e => e.ContragentId);

    }
}