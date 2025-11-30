using AutoFixture;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Queries;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using static Sai.DealAssistant.Application.Entities.SampleCustomers.Queries.GetSampleCustomersForAccountingQuery;

namespace Sai.DealAssistant.Application.Tests.SampleCustomers.Handlers
{
	public class GetSampleCustomersForAccountingQuery_Handler_Test : UnitTestBase
	{
		private ReadRepository<SampleCustomer> _customerRepositary;

		public GetSampleCustomersForAccountingQuery_Handler_Test()
			: base(seedTestData: false)
		{
			_customerRepositary = new ReadRepository<SampleCustomer>(DbContext);
			using (var db = CreateNewDbContext())
			{
				Fixture.Customize<SampleCustomer>(p => p.Without(c => c.Id).Without(c => c.Employees));
				var customers = Fixture.CreateMany<SampleCustomer>(100);
				int i = 0;
				string[] countries = { "US", "GB", "PL" };
				foreach (var customer in customers)
				{
					string country = countries[i++ % countries.Length];
					customer.Code = $"{country}{i}";
					customer.Country = country;
				}
				db.SampleCustomers.AddRange(customers);
				db.SaveChanges();
			}
		}

		[Fact]
		public async void GetSampleCustomersForAccountingQuery_Handler_SelectUsReturnsUs()
		{
			// Arrange
			string firstCountry = DbContext.SampleCustomers.First().Country;
			List<SampleCustomerForAccountingDto> benchmarkCustomersList = DbContext.SampleCustomers
				.Where(p => p.Country == firstCountry)
				.Select(p => new SampleCustomerForAccountingDto
				{
					Id = p.Id,
					Code = p.Code,
					Name = p.Name
				}).ToList();
			// Act
			var handler = new Handler(_customerRepositary);
			var command = new GetSampleCustomersForAccountingQuery { Country = firstCountry };
			var result = await handler.Handle(command, CancellationToken.None);
			// Assert
			Assert.True(benchmarkCustomersList.Count > 0, $"No {firstCountry} customers found");
			Assert.Equal(benchmarkCustomersList.Count, result.Items.Count);
			Assert.Equivalent(benchmarkCustomersList, result.Items);
		}

		[Fact]
		public async void GetSampleCustomersForAccountingQuery_Handler_ReturnsAllForEmptyCountry()
		{
			// Arrange
			List<SampleCustomerForAccountingDto> benchmarkCustomersList = DbContext.SampleCustomers
				.Select(p => new SampleCustomerForAccountingDto
				{
					Id = p.Id,
					Code = p.Code,
					Name = p.Name
				}).ToList();
			// Act
			var handler = new Handler(_customerRepositary);
			var command = new GetSampleCustomersForAccountingQuery { Country = string.Empty };
			var result = await handler.Handle(command, CancellationToken.None);
			// Assert
			Assert.Equal(benchmarkCustomersList.Count, result.TotalItems);
			Assert.Equal(benchmarkCustomersList.Count, result.Items.Count);
			Assert.Equivalent(benchmarkCustomersList, result.Items);
		}

		[Fact]
		public async void GetSampleCustomersForAccountingQuery_Handler_ReturnsEmptyDsForNonExistingCountry()
		{
			// Arrange
			// Act
			var handler = new Handler(_customerRepositary);
			var command = new GetSampleCustomersForAccountingQuery { Country = "Non existing Country" };
			var result = await handler.Handle(command, CancellationToken.None);
			// Assert
			Assert.Equal(0, result.TotalItems);
			Assert.Empty(result.Items);
		}
	}
}
