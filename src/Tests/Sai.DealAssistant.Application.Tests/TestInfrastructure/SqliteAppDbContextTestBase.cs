using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Infrastructure.Persistence;

namespace Sai.DealAssistant.Application.Tests.TestInfrastructure
{
    public abstract class SqliteAppDbContextTestBase : IDisposable
    {
        protected readonly SqliteConnection Connection;
        protected readonly AppDbContext DbContext;

        protected SqliteAppDbContextTestBase()
        {
            Connection = new SqliteConnection("DataSource=:memory:");
            Connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(Connection)
                .Options;

            DbContext = new AppDbContext(options);
            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            DbContext.Dispose();
            Connection.Dispose();
        }
    }
}