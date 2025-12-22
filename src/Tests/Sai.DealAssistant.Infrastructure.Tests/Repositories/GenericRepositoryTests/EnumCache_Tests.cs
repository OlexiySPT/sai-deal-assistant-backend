using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;

namespace Sai.DealAssistant.Infrastructure.Tests.Repositories.GenericRepositoryTests;

public class EnumCache_Tests : GenericRepoTestUnitTestBase
{
	private readonly ReadRepository<SampleEnum> _readRepository;

	public EnumCache_Tests()
		: base(seedTestData: false)
	{
		_readRepository = new ReadRepository<SampleEnum>(DbContext);
	}

	[Fact]
	public async Task GetAllAsync_ReturnsSeededDealTypes()
	{
		// Arrange: seed test enums into a fresh context instance
		using (var db = CreateNewDbContext())
		{
			var types = new List<SampleEnum>
			{
				new SampleEnum { /* Id will be DB-generated */ State = "One-time Service" },
				new SampleEnum { State = "Series" },
				new SampleEnum { State = "Long-time Collaboration" }
			};
			db.SampleEnums.AddRange(types);
			db.SaveChanges();
		}

		var cache = new EnumCache<SampleEnum>(_readRepository);
		// ensure clean start if previous tests populated static cache
		cache.Invalidate();

		// Act
		var items = await cache.GetAllAsync();

		// Assert
		Assert.NotNull(items);
		Assert.Equal(3, items.Count);
		Assert.Contains(items, t => t.State == "One-time Service");
		Assert.Contains(items, t => t.State == "Series");
		Assert.Contains(items, t => t.State == "Long-time Collaboration");
	}

	[Fact]
	public async Task GetAllAsync_CachesResultsAcrossCalls()
	{
		// Arrange: seed initial data
		using (var db = CreateNewDbContext())
		{
			db.SampleEnums.AddRange(new[]
			{
				new SampleEnum { State = "One-time Service" },
				new SampleEnum { State = "Series" }
            });
			db.SaveChanges();
		}

		var cache = new EnumCache<SampleEnum>(_readRepository);
		cache.Invalidate();

		// First call loads from DB
		var first = await cache.GetAllAsync();

		// Mutate DB after first load
		using (var db = CreateNewDbContext())
		{
			db.SampleEnums.Add(new SampleEnum { State = "Long-time Collaboration"});
			db.SaveChanges();
		}

		// Second call should return cached data (no new item)
		var second = await cache.GetAllAsync();

		// Assert: cached collection didn't pick up the new DB row
		Assert.Equal(first.Count, second.Count);
		Assert.DoesNotContain(second, t => t.State == "Long-time Collaboration");

		// Also verify the cache returned the same reference (cached instance)
		Assert.True(ReferenceEquals(first, second));
	}
}