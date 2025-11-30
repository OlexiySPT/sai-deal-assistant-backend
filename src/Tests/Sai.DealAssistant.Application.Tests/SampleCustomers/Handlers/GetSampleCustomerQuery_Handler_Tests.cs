using AutoFixture;
using AutoMapper;
using Moq;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Queries;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories;
using SAI.DealAssistant.TestUtils.Unit;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Sai.DealAssistant.Application.Entities.SampleCustomers.Queries.GetSampleCustomerQuery;
using static Sai.DealAssistant.Application.Entities.SampleCustomers.Queries.GetSampleCustomerQuery.Handler;

namespace Sai.DealAssistant.Application.Tests.SampleCustomers.Handlers
{
	public class GetSampleCustomerQuery_Handler_Tests : UnitTestBase
	{
		private readonly Mock<ISampleCustomerRepository> _customerRepositoryMock;

		// This autoMapper is serves as an example.
		private readonly Mock<IMapper> _mapperMock;

		public GetSampleCustomerQuery_Handler_Tests()
			: base(seedTestData: false)
		{
			_mapperMock = new Mock<IMapper>();
			_customerRepositoryMock = new Mock<ISampleCustomerRepository>();
		}
		/*
		[Fact]
		public async Task GetSampleCustomerQuery_Handler_ReturnsCustomer()
		{
			// Arrange
			var sampleCustomer = Fixture.Create<SampleCustomer>();

			_customerRepositoryMock
				.Setup(r => r.GetAsync(sampleCustomer.Id))
				.ReturnsAsync(sampleCustomer);
			_mapperMock.Setup(p => p.Map<SampleCustomerDto>(sampleCustomer))
				.Returns(new SampleCustomerDto { Id = sampleCustomer.Id, Code = sampleCustomer.Code, Name = sampleCustomer.Name });

			// Act
			var query = new GetSampleCustomerQuery(sampleCustomer.Id);
			SampleCustomerDto customer = await Handler.Handle(query, CancellationToken.None);

			// Assert
			Assert.NotNull(customer);
		}
		*/
	}
}
