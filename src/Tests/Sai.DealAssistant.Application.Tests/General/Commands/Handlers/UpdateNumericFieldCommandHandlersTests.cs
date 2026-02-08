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
    public class UpdateNumericFieldCommandHandlersTests : UnitTestBase
    {
        private readonly IFieldUpdateRepository<decimal?> _repo;

        public UpdateNumericFieldCommandHandlersTests()
            : base(seedTestData: true)
        {
            _repo = new FieldUpdateRepository<AppDbContext, decimal?>(DbContext);
        }

        [Fact]
        public async Task Handler_UpdatesNumericField_Success()
        {
            var deal = DbContext.Deals.First();
            var command = new UpdateNumericFieldCommand
            {
                Entity = "Deal",
                Field = "ProposalAmount",
                Id = deal.Id,
                Value = 12345.67m,
                NotNull = true
            };

            var handler = new UpdateNumericFieldCommand.Handler(_repo);
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Equal(12345.67m, result);
            var updated = DbContext.Deals.Find(deal.Id);
            Assert.Equal(12345.67m, updated.ProposalAmount);
        }

        [Fact]
        public async Task Handler_TableNotExists_Throws()
        {
            var command = new UpdateNumericFieldCommand
            {
                Entity = "NonExistingEntity",
                Field = "ProposalAmount",
                Id = 1,
                Value = 1m,
                NotNull = false
            };

            var handler = new UpdateNumericFieldCommand.Handler(_repo);
            await Assert.ThrowsAsync<TableNotExistsException>(
                () => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handler_ColumnNotExists_Throws()
        {
            var deal = DbContext.Deals.First();
            var command = new UpdateNumericFieldCommand
            {
                Entity = "Deal",
                Field = "NonExistingField",
                Id = deal.Id,
                Value = 1m,
                NotNull = false
            };

            var handler = new UpdateNumericFieldCommand.Handler(_repo);
            await Assert.ThrowsAsync<ColumnNotExistsException>(
                () => handler.Handle(command, CancellationToken.None));
        }
    }
}