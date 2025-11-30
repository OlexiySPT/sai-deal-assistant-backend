using AutoFixture;
using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Sai.DealAssistant.Application.System.Seeding;
using Sai.DealAssistant.Infrastructure.Persistence;

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
			LoggerFactory = new LoggerFactory();
			if (seedTestData)
			{
				SeedData();
			}

			DbContext = CreateNewDbContext();

			Fixture = new Fixture();
			// Necessary for ommitting recursion behaviour - https://chsamii.medium.com/autofixture-throwingrecursionbehavior-22918cc7dae7
			Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
				.ForEach(b => Fixture.Behaviors.Remove(b));
			Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

			//Mapper = PersistenceMappingProfile.CreateInstance();
			// TODO: Add IMediator ptop Init it properly
		}

		protected SqliteConnection SqliteDbConnection { get; }

		protected AppDbContext DbContext { get; }

		protected Fixture Fixture { get; }

		protected IMapper Mapper { get; }

		protected ILoggerFactory LoggerFactory { get; }

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

		protected void SeedData()
		{
			using (var seedDbContext = AppDbContextUtil_SQLiteDb.CreateAppDbContext(SqliteDbConnection))
			{
				var customers = DatabaseSeeder.GetCustomers().ToArray();
				var employees = DatabaseSeeder.GetEmployees();
				var custCount = customers.Length;
				int i = 0;
				foreach (var employee in employees)
				{
					customers[i++].Employees.Add(employee);
					if (i == custCount)
					{
						i = 0;
					}
				}

				seedDbContext.SampleCustomers.AddRange(customers);
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
}
