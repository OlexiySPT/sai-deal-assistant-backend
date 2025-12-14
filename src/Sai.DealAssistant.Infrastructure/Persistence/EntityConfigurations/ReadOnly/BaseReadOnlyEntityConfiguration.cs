using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities.ReadOnly;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations.ReadOnly;

public abstract class BaseReadOnlyEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseReadOnlyEntity
{
    virtual public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Id configuration
        builder.HasKey(c => c.Id);
    }
}
