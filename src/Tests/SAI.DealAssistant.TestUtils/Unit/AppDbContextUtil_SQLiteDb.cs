using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Infrastructure.Persistence;

namespace SAI.DealAssistant.TestUtils.Unit
{
	public class AppDbContextUtil_SQLiteDb
	{
		public static SqliteConnection CreateSqliteConnection()
		{
			SqliteConnection result = new SqliteConnection("DataSource=:memory:");
			result.Open();
			return result;
		}

		public static AppDbContext CreateAppDbContext(SqliteConnection connection)
		{
			DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
				.UseSqlite(connection)
#if DEBUG
				.EnableSensitiveDataLogging()
#endif
				.Options;

			AppDbContext result = new AppDbContext(options);
			result.Database.EnsureCreated();
			return result;
		}
	}
}
