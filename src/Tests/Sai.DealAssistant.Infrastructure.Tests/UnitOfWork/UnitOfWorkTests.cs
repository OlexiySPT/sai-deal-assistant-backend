using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities.Samples;
using SAI.DealAssistant.TestUtils.Unit;
using Sai.DealAssistant.Infrastructure;
using Sai.DealAssistant.Domain;

namespace Sai.DealAssistant.Infrastructure.Tests.UnitOfWork
{
	public class UnitOfWorkTests : GenericRepoTestUnitTestBase
	{
		public UnitOfWorkTests()
			: base(true)
		{
		}

		[Fact]
		public async Task ExecuteResilientTransactionAsync_InputActionCompletesSuccessfully_BeginsAndCommitsTransaction()
		{
            // Arrange
            IUnitOfWork unitOfWork = new Infrastructure.UnitOfWork(DbContext);
			SampleCustomer testCustomer1 = new SampleCustomer { Code = "1234", Name = "Test 1" };
			SampleCustomer testCustomer2 = new SampleCustomer { Code = "1111", Name = "Test 2" };
			Func<Task> action = async () =>
			{
				await DbContext.SampleCustomers.AddAsync(testCustomer1);
				await DbContext.SampleCustomers.AddAsync(testCustomer2);
				await DbContext.SaveChangesAsync();
			};

			// Act
			await unitOfWork.ExecuteResilientTransactionAsync(action, CancellationToken.None);

			// Assert
			SampleCustomer actualTestCountry1 = await DbContext.SampleCustomers.FirstOrDefaultAsync(
				c => c.Code == testCustomer1.Code);
			SampleCustomer actualTestCountry2 = await DbContext.SampleCustomers.FirstOrDefaultAsync(
				c => c.Code == testCustomer2.Code);
			Assert.NotNull(actualTestCountry1);
			Assert.NotNull(actualTestCountry2);
		}
	}
}
