using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities.Samples;
using Sai.DealAssistant.Domain.Exceptions;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Common;
using SAI.DealAssistant.TestUtils.Unit;
using SAI.DealAssistant.TestUtils.Unit.GenericRepositoryTests.Persistance;

namespace Sai.DealAssistant.Infrastructure.Tests.Repositories.GenericRepositoryTests
{
	public class ReadRepositoryTests : GenericRepoTestUnitTestBase
    {
		private readonly IReadRepository<SampleEmployee> _repo;
		private int _pageSize = 5;

		public ReadRepositoryTests()
			: base(seedTestData: true)
		{
			_repo = new ReadRepository<GenericRepoTestDbContext, SampleEmployee>(DbContext);
		}

		#region SelectAllWithFilters
		[Fact]
		public async void GetAll_ReturnsAll()
		{
			// Arrange
			var source = DbContext.SampleEmployees.ToList();

			// Act
			List<SampleEmployee> result = await _repo.GetAll().ToListAsync();

			// Assert
			CheckSourceDbSetIsNotEmpty();
			ComparisonHelpers.AssertSampleEmployeeListEqualToSource(source, result);
		}

		[Fact]
		public async void GetAll_FilteredByCustomerId()
		{
			// Arrange
			var customer = DbContext.SampleCustomers.First();
			var source = DbContext.SampleEmployees.Where(p => p.CustomerId == customer.Id).ToList();

			// Act
			List<SampleEmployee> result = await _repo.GetAll().Where(p => p.CustomerId == customer.Id).ToListAsync();

			// Assert
			CheckSourceDbSetIsNotEmpty();
			ComparisonHelpers.AssertSampleEmployeeListEqualToSource(source, result);
		}

		[Fact]
		public async void GetAll_FilteredBySecondNameThanByEmail()
		{
			// Arrange
			string lastNameFilter = "Novot";
			string emailFilter = "Novotny";
			var source = DbContext.SampleEmployees
				.Where(p => p.LastName.Contains(lastNameFilter)).Where(p => p.Email.Contains(emailFilter)).ToList();

			// Act
			var result = await _repo.GetAll()
				.Where(p => p.LastName.Contains(lastNameFilter)).Where(p => p.Email.Contains(emailFilter)).ToListAsync();

			// Assert
			CheckSourceDbSetIsNotEmpty();
			ComparisonHelpers.AssertSampleEmployeeListEqualToSource(source, result);
		}
		#endregion

		#region SelectAll_Filters_Select
		[Fact]
		public async void SelectAsync_ReturnsAll()
		{
			// Arrange
			var source = DbContext.SampleEmployees.Select(x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" }).ToList();

			// Act
			IQueryable<SampleEmployee> qry = _repo.GetAll();
			IEnumerable<SampleEmployeeTestDto> result = await
				_repo.SelectAsync(qry, x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" });

			// Assert
			CheckSourceDbSetIsNotEmpty();
			AssertSampleEmployeeDtoResultEqualToSource(source, result);
		}

		[Fact]
		public async void SelectAsync_ReturnsFiltered()
		{
			// Arrange
			string lastNameFilter = "Novot";
			string emailFilter = "Novotny";
			var source = DbContext.SampleEmployees
				.Where(p => p.LastName.Contains(lastNameFilter)).Where(p => p.Email.Contains(emailFilter))
				.Select(x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" }).ToList();

			// Act
			IQueryable<SampleEmployee> qry = _repo.GetAll()
				.Where(p => p.LastName.Contains(lastNameFilter)).Where(p => p.Email.Contains(emailFilter));
			IEnumerable<SampleEmployeeTestDto> result = await _repo.SelectAsync(
				qry,
				x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" });

			// Assert
			CheckSourceDbSetIsNotEmpty();
			AssertSampleEmployeeDtoResultEqualToSource(source, result);
		}
		#endregion

		#region SelectPaged_Filters_Select
		[Fact]
		public async void SelectPagedAsync_ReturnsAll_SortDefault_FirstPage()
		{
			// Arrange
			var src = DbContext.SampleEmployees
				.OrderBy(p => p.Id)
				.Skip(0)
				.Take(_pageSize)
				.Select(x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" })
				.ToList();

			// Act
			IQueryable<SampleEmployee> qry = _repo.GetAll();
			IEnumerable<SampleEmployeeTestDto> result =
				await _repo.SelectPageAsync(
					qry,
					x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" },
					1,
					_pageSize);

			// Assert
			CheckSourceDbSetIsNotEmpty();
			AssertIdListsHaveTheSameOrder(
				src.Select(x => x.Id).ToList(),
				result.Select(x => x.Id).ToList());
			AssertSampleEmployeeDtoResultEqualToSource(src, result);
		}

		[Fact]
		public async void SelectPagedAsync_ReturnsFiltered_SortDefault_FirstPage()
		{
			// Arrange
			var src = DbContext.SampleEmployees
				.Where(p => p.FirstName != "Jenny")
				.OrderBy(p => p.Id)
				.Skip(0)
				.Take(_pageSize)
				.Select(x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" })
				.ToList();

			// Act
			IQueryable<SampleEmployee> qry = _repo.GetAll()
				.Where(p => p.FirstName != "Jenny");
			IEnumerable<SampleEmployeeTestDto> result =
				await _repo.SelectPageAsync(
					qry,
					x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" },
					1,
					_pageSize);

			// Assert
			CheckSourceDbSetIsNotEmpty();
			AssertIdListsHaveTheSameOrder(
				src.Select(x => x.Id).ToList(),
				result.Select(x => x.Id).ToList());
			AssertSampleEmployeeDtoResultEqualToSource(src, result);
		}

		[Fact]
		public async void SelectPagedAsync_ReturnsAll_SortDefault_LastPage()
		{
			// Arrange
			int lastPage = (int)Math.Floor((decimal)(DbContext.SampleEmployees.Count() / _pageSize));

			var src = DbContext.SampleEmployees
				.OrderBy(p => p.Id)
				.Skip((lastPage - 1) * _pageSize)
				.Take(_pageSize)
				.Select(x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" })
				.ToList();

			// Act
			IQueryable<SampleEmployee> qry = _repo.GetAll();
			IEnumerable<SampleEmployeeTestDto> result =
				await _repo.SelectPageAsync(
					qry,
					x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" },
					lastPage,
					_pageSize);

			// Assert
			CheckSourceDbSetIsNotEmpty();
			AssertIdListsHaveTheSameOrder(
				src.Select(x => x.Id).ToList(),
				result.Select(x => x.Id).ToList());
			AssertSampleEmployeeDtoResultEqualToSource(src, result);
		}

		[Fact]
		public async void SelectPagedAsync_ReturnsAll_SortByEmail_LastPage()
		{
			// Arrange
			int lastPage = (int)Math.Floor((decimal)(DbContext.SampleEmployees.Count() / _pageSize));

			var src = DbContext.SampleEmployees
				.OrderBy(p => p.Email)
				.Skip((lastPage - 1) * _pageSize)
				.Take(_pageSize)
				.Select(x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" })
				.ToList();

			// Act
			IQueryable<SampleEmployee> qry = _repo.GetAll();
			IEnumerable<SampleEmployeeTestDto> result =
				await _repo.SelectPageAsync(
					qry,
					x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" },
					lastPage,
					_pageSize,
					"email",
					false);

			// Assert
			CheckSourceDbSetIsNotEmpty();
			AssertIdListsHaveTheSameOrder(
				src.Select(x => x.Id).ToList(),
				result.Select(x => x.Id).ToList());
			AssertSampleEmployeeDtoResultEqualToSource(src, result);
		}

		[Fact]
		public async void SelectPagedAsync_ReturnsAll_SortByEmailDesc_LastPage()
		{
			// Arrange
			int lastPage = (int)Math.Floor((decimal)(DbContext.SampleEmployees.Count() / _pageSize));

			var src = DbContext.SampleEmployees
				.OrderByDescending(p => p.Email)
				.Skip((lastPage - 1) * _pageSize)
				.Take(_pageSize)
				.Select(x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" })
				.ToList();

			// Act
			IQueryable<SampleEmployee> qry = _repo.GetAll();
			IEnumerable<SampleEmployeeTestDto> result =
				await _repo.SelectPageAsync(
					qry,
					x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" },
					lastPage,
					_pageSize,
					"email",
					true);

			// Assert
			CheckSourceDbSetIsNotEmpty();
			AssertIdListsHaveTheSameOrder(
				src.Select(x => x.Id).ToList(),
				result.Select(x => x.Id).ToList());
			AssertSampleEmployeeDtoResultEqualToSource(src, result);
		}

		[Fact]
		public async void SelectPagedAsync_AfterLastPage_ShouldBeEmpty()
		{
			// Arrange
			int lastPage = (int)Math.Floor((decimal)(DbContext.SampleEmployees.Count() / _pageSize));

			// Act
			IQueryable<SampleEmployee> qry = _repo.GetAll();
			IEnumerable<SampleEmployeeTestDto> result =
				await _repo.SelectPageAsync(
					qry,
					x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" },
					lastPage + 2,
					_pageSize,
					"email",
					true);

			// Assert
			CheckSourceDbSetIsNotEmpty();
			Assert.Empty(result);
		}

		[Fact]
		public async void SelectPagedAsync_ThrowsInvalidSortColumnException()
		{
			// Assert
			await Assert.ThrowsAsync<InvalidSortColumnException>(async () =>
			{
				// Act
				IQueryable<SampleEmployee> qry = _repo.GetAll();
				var result = await _repo.SelectPageAsync(
					qry,
					x => new SampleEmployeeTestDto { Id = x.Id, Fullname = $"{x.FirstName} {x.LastName}" },
					1,
					_pageSize,
					"emqwertyuiop",
					false);
			});
		}
		#endregion

		#region CountAsync
		[Fact]
		public async void CountAsync_ReturnsAll()
		{
			// Arrange
			int srcCount = DbContext.SampleEmployees.Count();

			// Act
			IQueryable<SampleEmployee> qry = _repo.GetAll();
			int resultCount = await _repo.CountAsync(qry);

			// Assert
			CheckSourceDbSetIsNotEmpty();
			Assert.Equal(resultCount, srcCount);
		}

		[Fact]
		public async void CountAsync_ReturnsFiltered()
		{
			// Arrange
			string lastNameFilter = "Novot";
			string emailFilter = "Novotny";
			int srcCount = DbContext.SampleEmployees
				.Where(p => p.LastName.Contains(lastNameFilter)).Where(p => p.Email.Contains(emailFilter))
				.Count();

			// Act
			IQueryable<SampleEmployee> qry = _repo.GetAll()
				.Where(p => p.LastName.Contains(lastNameFilter)).Where(p => p.Email.Contains(emailFilter));
			int resultCount = await _repo.CountAsync(qry);

			// Assert
			CheckSourceDbSetIsNotEmpty();
			Assert.Equal(resultCount, srcCount);
		}
		#endregion

		#region ApplySorting
		[Fact]
		public async void ApplySorting_SortsAscending()
		{
			// Arrange
			var src = DbContext.SampleEmployees
				.OrderBy(p => p.Email).Select(p => p.Id).ToList();

			// Act
			IQueryable<SampleEmployee> qry = _repo.GetAll();
			var result = await _repo.ApplySorting(
				qry,
				"email",
				false)
				.Select(p => p.Id)
				.ToListAsync();

			// Assert
			CheckSourceDbSetIsNotEmpty();
			AssertIdListsHaveTheSameOrder(src, result);
		}

		[Fact]
		public async void ApplySorting_SortsDescending()
		{
			// Arrange
			var src = DbContext.SampleEmployees
				.OrderByDescending(p => p.Email).Select(p => p.Id).ToList();

			// Act
			IQueryable<SampleEmployee> qry = _repo.GetAll();
			var result = await _repo.ApplySorting(
				qry,
				"email",
				true)
				.Select(p => p.Id)
				.ToListAsync();

			// Assert
			CheckSourceDbSetIsNotEmpty();
			AssertIdListsHaveTheSameOrder(src, result);
		}

		[Fact]

		public void ApplySorting_ThrowsInvalidSortColumnException()
		{
			// Arrange
			var src = DbContext.SampleEmployees
				.OrderBy(p => p.Email).Select(p => p.Id).ToList();

			// Assert
			Assert.Throws<InvalidSortColumnException>(() =>
			{
				// Act
				IQueryable<SampleEmployee> qry = _repo.GetAll();
				var result = _repo.ApplySorting(
					qry,
					"emailqwert",
					false)
					.Select(p => p.Id)
					.ToListAsync();
			});
		}

		[Fact]
		public void ApplySorting_ThrowsArgumentNullException_ForColumn()
		{
			// Arrange
			var src = DbContext.SampleEmployees
				.OrderBy(p => p.Email).Select(p => p.Id).ToList();

			// Assert
			Assert.Throws<ArgumentNullException>(
				"column",
				() =>
				{
					// Act
					IQueryable<SampleEmployee> qry = _repo.GetAll();
					var result = _repo.ApplySorting(
						qry,
						null,
						false)
						.Select(p => p.Id)
						.ToListAsync();
				});
		}

		#endregion

		[Fact]
		public async void ExistsAsync_ReturnsTrue_ForExistingEntity()
		{
			// Arrange
			CheckSourceDbSetIsNotEmpty();
			var existingId = DbContext.SampleCustomers.First().Id;

			// Act
			var exists = await _repo.ExistsAsync(x => x.Id == existingId);

			// Assert
			Assert.True(exists);
		}

		[Fact]
		public async void ExistsAsync_ReturnsFalse_WhenItemDoesNotExist()
		{
			// Arrange
			CheckSourceDbSetIsNotEmpty();
			var testNonExistingId = DbContext.SampleEmployees.OrderByDescending(p => p.Id).First().Id + 100;

			// Act
			var exists = await _repo.ExistsAsync(x => x.Id == testNonExistingId);

			// Assert
			Assert.False(exists);
		}

		[Fact]
		public async Task FirstOrDefaultAsync_ReturnsItem_WhenItemExists()
		{
			// Arrange
			CheckSourceDbSetIsNotEmpty();
			var existingId = DbContext.SampleCustomers.First().Id;

			// Act
			var result = await _repo.FirstOrDefaultAsync(x => x.Id == existingId);

			// Assert
			Assert.NotNull(result);
		}

		[Fact]
		public async Task FirstOrDefaultAsync_ReturnsNull_WhenItemDoesNotExist()
		{
			// Arrange
			CheckSourceDbSetIsNotEmpty();
			var testNonExistingId = DbContext.SampleEmployees.OrderByDescending(p => p.Id).First().Id + 100;

			// Act
			var result = await _repo.FirstOrDefaultAsync(x => x.Id == testNonExistingId);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task SingleOrDefaultAsync_ReturnsItem_WhenItemExists()
		{
			// Arrange
			CheckSourceDbSetIsNotEmpty();
			var existingId = DbContext.SampleCustomers.First().Id;

			// Act
			var result = await _repo.SingleOrDefaultAsync(x => x.Id == existingId);

			// Assert
			Assert.NotNull(result);
		}

		#region Helpers
		private void CheckSourceDbSetIsNotEmpty()
		{
			Assert.True(DbContext.SampleEmployees.Count() > 0, "UnitTests should NOT be ran on the EMPTY DbSet");
		}

		private void AssertSampleEmployeeDtoResultEqualToSource(
			IEnumerable<SampleEmployeeTestDto> source, IEnumerable<SampleEmployeeTestDto> result)
		{
			Assert.True(source.Count() > 0);
			Assert.Equal(source.Count(), result.Count());
			foreach (SampleEmployeeTestDto employee in result)
			{
				SampleEmployeeTestDto src = source.FirstOrDefault(p => p.Id == employee.Id);
				Assert.NotNull(src);
				Assert.Equal(employee.Fullname, src.Fullname);
			}
		}

		private void AssertIdListsHaveTheSameOrder(List<int> src, List<int> result)
		{
			Assert.Equal(src.Count, result.Count);
			for (int i = 0; i < src.Count; i++)
			{
				Assert.Equal(src[i], result[i]);
			}
		}
		#endregion
	}

	public class SampleEmployeeTestDto
	{
		public int Id { get; set; }

		public string Fullname { get; set; }
	}
}
