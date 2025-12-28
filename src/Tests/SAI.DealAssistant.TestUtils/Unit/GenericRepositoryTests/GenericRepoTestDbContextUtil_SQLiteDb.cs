using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SAI.DealAssistant.TestUtils.Unit.GenericRepositoryTests.Persistance;

namespace SAI.DealAssistant.TestUtils.Unit
{
	public class GenericRepoTestDbContextUtil_SQLiteDb
	{
		public static SqliteConnection CreateSqliteConnection()
		{
			SqliteConnection result = new SqliteConnection("DataSource=:memory:");
			result.Open();
			return result;
		}

		public static GenericRepoTestDbContext CreateDbContext(SqliteConnection connection)
		{
			DbContextOptions<GenericRepoTestDbContext> options = new DbContextOptionsBuilder<GenericRepoTestDbContext>()
				.UseSqlite(connection)
#if DEBUG
				.EnableSensitiveDataLogging()
#endif
				.Options;

            var result = new GenericRepoTestDbContext(options);
			result.Database.EnsureCreated();
			return result;
		}
	}
}
