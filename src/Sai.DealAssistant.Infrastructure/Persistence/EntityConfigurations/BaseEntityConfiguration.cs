using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public abstract class BaseEntityConfiguration<TEntity> : BaseNonTrackedEntityConfiguration<TEntity>
    where TEntity : BaseEntity
{

    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        //Shadow prop for optimistic concurrency implementation
        builder.Property<uint>("xmin").IsRowVersion().HasDefaultValue(0);

        // Uncomment this to use soft delete via shadow property
        // and do not forget to update universal and specific repos and migrations
        //builder.Property<DateTime>("IsDeleted");
        //builder.HasQueryFilter(c => !EF.Property<bool>(c, "IsDeleted"));
    }
}
