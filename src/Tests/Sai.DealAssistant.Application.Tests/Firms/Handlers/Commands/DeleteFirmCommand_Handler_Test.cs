using AutoMapper;
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
	public class DeleteFirmCommand_Handler_Test : UnitTestBase
	{
		private readonly CrudRepository<AppDbContext, Firm> _firmRepository;
		private readonly IMapper _mapper;

		public DeleteFirmCommand_Handler_Test()
			: base(seedTestData: false)
		{
			_firmRepository = new CrudRepository<AppDbContext, Firm>(DbContext);

			var cfg = new MapperConfiguration(c =>
			{
				c.AddProfile<FirmDto.MappingProfile>();
			}, LoggerFactory);
			_mapper = cfg.CreateMapper();

			// Seed test data
			using (var db = CreateNewDbContext())
			{
				db.Firms.Add(new Firm
				{
					Name = "Firm To Delete",
					Country = "Italy",
					Description = "Will be deleted"
				});
				db.SaveChanges();
			}
		}

		[Fact]
		public async void Handler_DeletesFirm_ReturnsDeletedDto()
		{
			// Arrange
			var handler = new DeleteFirmCommand.Handler(_firmRepository, _mapper);
			var existing = DbContext.Firms.First();

			// Act
			var result = await handler.Handle(new DeleteFirmCommand(existing.Id), CancellationToken.None);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(existing.Id, result.Id);
			Assert.Equal(existing.Name, result.Name);
			Assert.Equal(existing.Country, result.Country);

			// Verify removed from DB
			var deleted = DbContext.Firms.SingleOrDefault(f => f.Id == existing.Id);
			Assert.Null(deleted);
		}

		[Fact]
		public async void Handler_ThrowsNotFoundException_WhenFirmDoesNotExist()
		{
			// Arrange
			var handler = new DeleteFirmCommand.Handler(_firmRepository, _mapper);
			var nonExistingId = DbContext.Firms.OrderByDescending(f => f.Id).First().Id + 100;

			// Act / Assert
			await Assert.ThrowsAsync<NotFoundExceptionOverride>(async () =>
			{
				await handler.Handle(new DeleteFirmCommand(nonExistingId), CancellationToken.None);
			});
		}
	}
}