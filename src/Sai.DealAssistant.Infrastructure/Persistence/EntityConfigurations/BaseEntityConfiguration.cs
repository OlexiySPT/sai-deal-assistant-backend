using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sai.DealAssistant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sai.DealAssistant.Infrastructure.Persistence.EntityConfigurations;

public class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{

    virtual public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Id configuration
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        //Shadow prop for optimistic concurrency implementation
        builder.Property<uint>("xmin").IsRowVersion();
    }
}
