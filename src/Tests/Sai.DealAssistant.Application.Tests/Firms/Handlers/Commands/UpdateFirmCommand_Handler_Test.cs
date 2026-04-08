using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Common.Exceptions;
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
	public class UpdateFirmCommand_Handler_Test : UnitTestBase
	{
		private readonly CrudRepository<AppDbContext, Firm> _firmRepository;
		private readonly IMapper _mapper;

		public UpdateFirmCommand_Handler_Test()
			: base(seedTestData: false)
		{
			_firmRepository = new CrudRepository<AppDbContext, Firm>(DbContext);

			var cfg = new MapperConfiguration(c =>
			{
				c.AddProfile<FirmDto.MappingProfile>();
				c.AddProfile<UpdateFirmCommand.Handler.MappingProfile>();
			}, LoggerFactory);
			_mapper = cfg.CreateMapper();

			// Seed test data
			using (var db = CreateNewDbContext())
			{
				db.Firms.Add(new Firm
				{
					Name = "Original Firm",
					Country = "USA",
					Description = "Original description"
				});
				db.SaveChanges();
			}
		}

		[Fact]
		public async void Handler_UpdatesFirm_ReturnsUpdatedDto()
		{
			// Arrange
			var handler = new UpdateFirmCommand.Handler(_firmRepository, _mapper);
				var existing = DbContext.Firms.AsNoTracking().First();

			var command = new UpdateFirmCommand
			{
				Id = existing.Id,
				Name = "Updated Firm",
				Country = "Germany",
				Description = "Updated description"
			};

			// Act
			var result = await handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(existing.Id, result.Id);
			Assert.Equal(command.Name, result.Name);
			Assert.Equal(command.Country, result.Country);
			Assert.Equal(command.Description, result.Description);
		}

		[Fact]
		public async void Handler_ThrowsNotFoundException_WhenFirmDoesNotExist()
		{
			// Arrange
			var handler = new UpdateFirmCommand.Handler(_firmRepository, _mapper);
			var nonExistingId = DbContext.Firms.OrderByDescending(f => f.Id).First().Id + 100;
			var command = new UpdateFirmCommand
			{
				Id = nonExistingId,
				Name = "Does Not Exist",
				Country = "Nowhere"
			};

			// Act / Assert
			await Assert.ThrowsAsync<NotFoundExceptionOverride>(async () =>
			{
				await handler.Handle(command, CancellationToken.None);
			});
		}
	}
}