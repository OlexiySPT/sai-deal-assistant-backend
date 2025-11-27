using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Common;
using SAI.DealAssistant.TestUtils.Unit;

namespace Sai.DealAssistant.Infrastructure.Tests.Repositories.GenericRepositoryTests
{
	public class CrudRepositoryTests : DealAsistantUnitTestBase
	{
		private const int NonExistingId = -1;
		private readonly ICrudRepository<SampleEmployee> _repo;
		private SampleCustomer _customer;

		public CrudRepositoryTests()
			: base(seedTestData: true)
		{
			_repo = new CrudRepository<SampleEmployee>(DbContext);
			_customer = DbContext.SampleCustomers.First();
			if (_customer == null)
			{
				using (var dbContext = CreateNewDbContext())
				{
					_customer = Fixture.Create<SampleCustomer>();
					dbContext.SampleCustomers.Add(_customer);
					dbContext.SaveChanges();
				}
			}

			Fixture.Customize<SampleEmployee>(p => p.With(c => c.CustomerId, _customer.Id).Without(c => c.Id).Without(c => c.Customer));
		}

		[Fact]
		public async void CreateAsync_CreatesSampleEmployee()
		{
			// Arrange
			var newEmployee = Fixture.Create<SampleEmployee>();
			// Act
			var result = await _repo.CreateAsync(newEmployee);
			// Assert
			Assert.True(result.Id > 0);
			var insertedRow = await DbContext.SampleEmployees.FirstOrDefaultAsync(p => p.Id == result.Id);
			Assert.NotNull(insertedRow);
			Assert.Equivalent(newEmployee, insertedRow);
		}

		[Fact]
		public async void UpdateAsync_WithChangedName_UpdatesSampleEmployee()
		{
			// Arrange
			var newEmployee = Fixture.Create<SampleEmployee>();
			string newEmployeeName = Fixture.Create<string>();

			DbContext.SampleEmployees.Add(newEmployee);
			DbContext.SaveChanges();

			newEmployee.FirstName = newEmployeeName;

			// Act
			var result = await _repo.UpdateAsync(newEmployee);

			// Assert
			Assert.True(result.Id > 0);
			var updatedRow = await DbContext.SampleEmployees.FirstOrDefaultAsync(p => p.Id == result.Id);
			Assert.NotNull(updatedRow);
            updatedRow.AssertEqualsTo(newEmployee);
            Assert.Equal(newEmployeeName, newEmployee.FirstName);
			Assert.Equivalent(newEmployee, updatedRow);
		}

		[Fact]
		public async void DeleteAsync_DeletesSampleEmployeeById()
		{
			// Arrange
			var newEmployee = Fixture.Create<SampleEmployee>();
			DbContext.SampleEmployees.Add(newEmployee);
			DbContext.SaveChanges();

			// Act
			var result = await _repo.DeleteAsync(newEmployee.Id);

			// Assert
			Assert.True(result.Id > 0);
			var deletedRow = await DbContext.SampleEmployees.FirstOrDefaultAsync(p => p.Id == result.Id);
			Assert.Null(deletedRow);
		}

		[Fact]
		public async void DeleteAsync_ForNonExistingId_ReturnsNull()
		{
			// Act
			var result = await _repo.DeleteAsync(NonExistingId);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async void DeleteAsync_DeletesSampleEmployee()
		{
			// Arrange
			var newEmployee = Fixture.Create<SampleEmployee>();
			DbContext.SampleEmployees.Add(newEmployee);
			DbContext.SaveChanges();

			// Act
			var result = await _repo.DeleteAsync(newEmployee);

			// Assert
			Assert.True(result.Id > 0);
			var deletedRow = await DbContext.SampleEmployees.FirstOrDefaultAsync(p => p.Id == result.Id);
			Assert.Null(deletedRow);
		}
	}
}
