using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public abstract class BaseNonTrackedEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseNonTrackedEntity
{
    virtual public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Id configuration
        builder.HasKey(c => c.Id);
    }
}
