using AutoFixture;
using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Sai.DealAssistant.Application.Entities.Deals.Dtos;
using Sai.DealAssistant.Application.System.Seeding;
using Sai.DealAssistant.Common.Configuration;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories;

namespace SAI.DealAssistant.TestUtils.Unit
{
	/// <summary>
	/// Base class for UnitTests.
	/// Contains mapper and seeds data using SeedRepository.
	/// </summary>
	public class UnitTestBase : IDisposable
	{
		private bool _disposedValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="UnitTestBase"/> class.
		/// Initializes DbContext, Fixture, Mapper and LoggerFactory.
		/// </summary>
		/// <param name="seedTestData">Flag if test data should be seeded.</param>
		public UnitTestBase(bool seedTestData)
		{
			SqliteDbConnection = AppDbContextUtil_SQLiteDb.CreateSqliteConnection();
            if (seedTestData)
			{		
				SeedDataAsync(seedTestData).GetAwaiter().GetResult();
			}

			DbContext = CreateNewDbContext();

			Fixture = new Fixture();
			// Necessary for ommitting recursion behaviour - https://chsamii.medium.com/autofixture-throwingrecursionbehavior-22918cc7dae7
			Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
				.ForEach(b => Fixture.Behaviors.Remove(b));
			Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

			// Register all AutoMapper profiles from the Application assembly, with a fallback to the known profile.
			var config = new MapperConfiguration(cfg =>
			{
				// Try to load the Application assembly and register any Profile types it contains.
				try
				{
					var appAssembly = typeof(DealDto).Assembly;
					if (appAssembly != null)
					{
						cfg.AddMaps(appAssembly);
						return;
					}
				}
				catch
				{
					// ignore and fallback to explicit registration below
				}
			},
			LoggerFactory);
			Mapper = config.CreateMapper();
			// TODO: Add IMediator ptop Init it properly
		}

		protected SqliteConnection SqliteDbConnection { get; }

		protected AppDbContext DbContext { get; }

		protected Fixture Fixture { get; }

		protected IMapper Mapper { get; }

		protected ILoggerFactory LoggerFactory { get; } = new LoggerFactory();

        public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected AppDbContext CreateNewDbContext()
		{
			return AppDbContextUtil_SQLiteDb.CreateAppDbContext(SqliteDbConnection);
		}

		protected async Task SeedDataAsync(bool seedTestData)
		{
			using (var seedDbContext = AppDbContextUtil_SQLiteDb.CreateAppDbContext(SqliteDbConnection))
			{
				var seedRepo = new SeedRepository(LoggerFactory.CreateLogger<SeedRepository>(), seedDbContext, new TestAppConfiguration());
				var databaseSeeder = new DatabaseSeeder(LoggerFactory.CreateLogger<DatabaseSeeder>(), seedRepo);
				await databaseSeeder.SeedAsync();
				if(seedTestData)
				{
					await databaseSeeder.SeedTestDataAsync();
				}
				seedDbContext.SaveChanges();
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					DbContext.Dispose();
					SqliteDbConnection.Dispose();
				}

				_disposedValue = true;
			}
		}
	}

	/// <summary>Minimal IAppConfiguration for use in unit tests.</summary>
	internal sealed class TestAppConfiguration : IAppConfiguration
	{
		public string AppConnectionString       => string.Empty;
		public string MigrationConnectionString => string.Empty;
		public string AllowedCorsOrigins        => string.Empty;
		public int    EnumTablesCacheExpitrationMins => 10;
		public int    DefaultResultPageSize     => 10;
		public bool   SeedTestData              => false;
		public int    MultiplyDealsTargetRowCount => 0;
		public int    BulkSqlTimeoutSeconds     => 900;
	}
}
