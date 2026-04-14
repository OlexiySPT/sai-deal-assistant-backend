using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Sai.DealAssistant.Domain.Exceptions;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Sai.DealAssistant.Infrastructure.Repositories.Generic
{
    public class FieldUpdateRepository<TDbContext, TFieldType> : IFieldUpdateRepository<TFieldType>
        where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;

        public FieldUpdateRepository(TDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public bool TableExists(string entity)
        {
            return _dbContext.Model.GetEntityTypes()
                .Any(e => string.Equals(e.ClrType.Name, entity, StringComparison.OrdinalIgnoreCase));
        }

        public bool ColumnExists(string entity, string field)
        {
            var entityType = _dbContext.Model.GetEntityTypes()
                .FirstOrDefault(e => string.Equals(e.ClrType.Name, entity, StringComparison.OrdinalIgnoreCase));
            if (entityType == null)
                return false;

            var property = entityType.ClrType.GetProperty(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            return property != null;
        }

        public async Task UpdateFieldAsync(string entity, string field, int id, TFieldType? newValue)
        {
            var entityType = _dbContext.Model.GetEntityTypes()
                .FirstOrDefault(e => string.Equals(e.ClrType.Name, entity, StringComparison.OrdinalIgnoreCase));
            if (entityType == null)
                throw new TableNotExistsException(entity);

            var property = entityType.ClrType.GetProperty(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property == null)
                throw new ColumnNotExistsException(field);

            var keyProperty = entityType.FindPrimaryKey()?.Properties.FirstOrDefault();
            if (keyProperty == null)
                throw new InvalidOperationException($"No primary key defined for entity '{entity}'.");

            // In theory, parameterized SQL is a great solution from the performance perspective
            //
            // var sql = $"UPDATE \"{tableName}\" SET \"{columnName}\" = @p0 WHERE \"{keyColumn}\" = @p1";
            // await _dbContext.Database.ExecuteSqlRawAsync(sql, newValue, id);
            //
            // However, in this case different kinds of global filters, etc. needed to be added manually to the SQL
            // , which is not a good idea. So, we will just load the entity, update the field and save changes.


            //Update itself
            var dbSet = (IQueryable<object>)_dbContext.GetType()
                .GetMethod("Set", Type.EmptyTypes)!
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(_dbContext, null)!;
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var propertyAccess = Expression.Property(parameter, keyProperty.Name);
            var idValue = Expression.Constant(Convert.ChangeType(id, propertyAccess.Type));
            var equals = Expression.Equal(propertyAccess, idValue);
            var lambda = Expression.Lambda(equals, parameter);

            var whereMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType.ClrType);
            var query = whereMethod.Invoke(null, new object[] { dbSet, lambda });

            var firstOrDefaultAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethods()
                .First(m => m.Name == "FirstOrDefaultAsync" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType.ClrType);
            var entityObjTask = (Task)firstOrDefaultAsyncMethod.Invoke(null, new object[] { query, default(System.Threading.CancellationToken) });
            await entityObjTask.ConfigureAwait(false);
            var entityObj = entityObjTask.GetType().GetProperty("Result")?.GetValue(entityObjTask);

            if (entityObj == null)
                throw new EntityNotFoundException($"No record found in table '{entity}' with id '{id}'.");

            var targetType = property.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            object? convertedValue = newValue;
            if (newValue != null && newValue.GetType() != targetType)
            {
                convertedValue = Convert.ChangeType(newValue, underlyingType);
            }
            property.SetValue(entityObj, convertedValue);

            await _dbContext.SaveChangesAsync();
        }
    }
}