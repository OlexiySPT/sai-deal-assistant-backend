using Microsoft.EntityFrameworkCore;
using Moq;
using Sai.DealAssistant.Application.Common.Caching;
using Sai.DealAssistant.Domain.Entities.Samples;
using Sai.DealAssistant.Domain.Repositories;
using Sai.DealAssistant.Infrastructure.Repositories;
using SAI.DealAssistant.TestUtils.Unit;

namespace Sai.DealAssistant.Infrastructure.Tests.Repositories
{
	public class CustomerRepositoryTests : UnitTestBase
    {
		private readonly Mock<IMemoryCacheService> _cacheMock;
		private ISampleCustomerRepository _repository;

		public CustomerRepositoryTests()
			: base(seedTestData: true)
		{
			_cacheMock = new Mock<IMemoryCacheService>();

			_repository = new SampleCustomerRepository(DbContext/*, Mapper*/);
			SampleCustomer? customer = null;

			_cacheMock
				.Setup(x => x.TryGetValue(It.IsAny<string>(), out customer))
				.Returns(false);
		}

		[Fact]
		public async Task GetAsyncReturnsCustomers()
		{
			IReadOnlyCollection<SampleCustomer> customers = await _repository.GetAsync();

			Assert.NotNull(customers);
			Assert.NotEmpty(customers);
		}

		[Fact]
		public async Task GetAsync_ReturnsNull()
		{
			SampleCustomer customer = await _repository.GetAsync("000");

			Assert.Null(customer);
		}

		[Fact]
		public async Task GetAsync_ReturnsOneCustomer()
		{
			SampleCustomer customer = await _repository.GetAsync(1);

			Assert.NotNull(customer);
			Assert.Equal(1, customer.Id);
		}

		[Fact]
		public async Task GetAsync_ReturnsOneCustomerByCode()
		{
			SampleCustomer sample = DbContext.SampleCustomers.First();

			SampleCustomer customer = await _repository.GetAsync(sample?.Code);

			Assert.NotNull(customer);
			Assert.Equal(sample?.Code, customer.Code);
		}

		[Fact]
		public async Task CountAsync_ReturnsFive()
		{
			// Arrange
			int src = await DbContext.SampleCustomers.CountAsync();

			// Act
			int result = await _repository.CountAsync();

			// Assert
			Assert.Equal(src, result);
		}

		[Fact]
		public async Task CountAsyncReturnsOneByName()
		{
			// Arrange
			SampleCustomer sample = DbContext.SampleCustomers.First();

			// Name is not unique, more than one customet can contain the same name part
			int src = DbContext.SampleCustomers.Count(p => p.Name.Contains(sample.Name));

			// Act
			int result = await _repository.CountAsync(null, sample.Name);

			// Assert
			Assert.Equal(src, result);
		}

		[Fact]
		public async Task CreateAsync_ReturnsNewCustomerId()
		{
			SampleCustomer newCustomer = new SampleCustomer
			{
				Code = "ZZA",
				Name = "Customer A"
			};

			SampleCustomer customer = await _repository.CreateAsync(newCustomer);

			Assert.NotNull(customer);
			Assert.True(customer.Id > 0);

			SampleCustomer check = await DbContext.SampleCustomers.FirstAsync(p => p.Code == newCustomer.Code);

			Assert.NotNull(customer);
			Assert.Equal(newCustomer.Code, check.Code);
			Assert.Equal(newCustomer.Name, check.Name);
		}

		[Fact]
		public async Task UpdateAsync_UpdatesCustomer()
		{
			var customer = DbContext.SampleCustomers.FirstOrDefault();
			var id = customer.Id;
			var newName = "New Updated";

			customer.Name = newName;

			await _repository.UpdateAsync(customer);

			var updatedCustomer = await DbContext.SampleCustomers.FirstOrDefaultAsync(p => p.Id == id);
			Assert.NotNull(updatedCustomer);
			Assert.Equal(newName, updatedCustomer.Name);
			Assert.Equivalent(customer, updatedCustomer);
		}
	}
}
