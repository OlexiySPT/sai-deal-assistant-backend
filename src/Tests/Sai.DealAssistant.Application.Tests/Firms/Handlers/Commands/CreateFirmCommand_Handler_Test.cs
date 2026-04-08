using AutoMapper;
using Sai.DealAssistant.Application.Entities.Firms.Commands;
using Sai.DealAssistant.Application.Entities.Firms.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System.Linq;
using System.Threading;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Firms.Handlers.Commands
{
	public class CreateFirmCommand_Handler_Test : UnitTestBase
	{
		private readonly CrudRepository<AppDbContext, Firm> _firmRepository;
		private readonly IMapper _mapper;

		public CreateFirmCommand_Handler_Test()
			: base(seedTestData: false)
		{
			_firmRepository = new CrudRepository<AppDbContext, Firm>(DbContext);

			var cfg = new MapperConfiguration(c =>
			{
				c.AddProfile<FirmDto.MappingProfile>();
				c.AddProfile<CreateFirmCommand.Handler.MappingProfile>();
			}, LoggerFactory);
			_mapper = cfg.CreateMapper();
		}

		[Fact]
		public async void Handler_CreatesFirm_ReturnsDto()
		{
			// Arrange
			var handler = new CreateFirmCommand.Handler(_firmRepository, _mapper);
			var command = new CreateFirmCommand
			{
				Name = "New Firm",
				Country = "France",
				Description = "A test firm"
			};

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Id > 0);
			Assert.Equal(command.Name, result.Name);
			Assert.Equal(command.Country, result.Country);
			Assert.Equal(command.Description, result.Description);

			// Verify persisted
			var persisted = DbContext.Firms.SingleOrDefault(f => f.Id == result.Id);
			Assert.NotNull(persisted);
			Assert.Equal(command.Name, persisted.Name);
			Assert.Equal(command.Country, persisted.Country);
			Assert.Equal(command.Description, persisted.Description);
		}

		[Fact]
		public async void Handler_CreatesFirm_WithNullDescription()
		{
			// Arrange
			var handler = new CreateFirmCommand.Handler(_firmRepository, _mapper);
			var command = new CreateFirmCommand
			{
				Name = "Firm No Desc",
				Country = "Spain",
				Description = null
			};

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Id > 0);
			Assert.Equal(command.Name, result.Name);
			Assert.Equal(command.Country, result.Country);
			Assert.Null(result.Description);
		}
	}
}