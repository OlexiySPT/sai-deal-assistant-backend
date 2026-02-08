using AutoMapper;
using Sai.DealAssistant.Application.Entities.General.Commands;
using Sai.DealAssistant.Domain.Exceptions;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.General.Commands.Handlers
{
    public class UpdateStringFieldCommandHandlersTests : UnitTestBase
    {
        private readonly IFieldUpdateRepository<string> _repo;

        public UpdateStringFieldCommandHandlersTests()
            : base(seedTestData: true)
        {
            _repo = new FieldUpdateRepository<AppDbContext, string>(DbContext);
        }

        [Fact]
        public async Task Handler_UpdatesStringField_Success()
        {
            var deal = DbContext.Deals.First();
            var command = new UpdateStringFieldCommand
            {
                Entity = "Deal",
                Field = "Name",
                Id = deal.Id,
                Value = "UpdatedDealName",
                Validation = StringFieldValidationType.NotEmpty
            };

            var handler = new UpdateStringFieldCommand.Handler(_repo);
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Equal("UpdatedDealName", result);
            var updated = DbContext.Deals.Find(deal.Id);
            Assert.Equal("UpdatedDealName", updated.Name);
        }

        [Fact]
        public async Task Handler_TableNotExists_Throws()
        {
            var command = new UpdateStringFieldCommand
            {
                Entity = "NonExistingEntity",
                Field = "Name",
                Id = 1,
                Value = "Test",
                Validation = StringFieldValidationType.None
            };

            var handler = new UpdateStringFieldCommand.Handler(_repo);
            await Assert.ThrowsAsync<TableNotExistsException>(
                () => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handler_ColumnNotExists_Throws()
        {
            var deal = DbContext.Deals.First();
            var command = new UpdateStringFieldCommand
            {
                Entity = "Deal",
                Field = "NonExistingField",
                Id = deal.Id,
                Value = "Test",
                Validation = StringFieldValidationType.None
            };

            var handler = new UpdateStringFieldCommand.Handler(_repo);
            await Assert.ThrowsAsync<ColumnNotExistsException>(
                () => handler.Handle(command, CancellationToken.None));
        }
    }
}