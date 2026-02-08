using AutoMapper;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.General.Commands;
using Sai.DealAssistant.Domain.Exceptions;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.General.Commands.Handlers
{
    public class UpdateDateFieldCommandHandlersTests : UnitTestBase
    {
        private readonly IFieldUpdateRepository<DateTimeOffset?> _repo;

        public UpdateDateFieldCommandHandlersTests()
            : base(seedTestData: true)
        {
            _repo = new FieldUpdateRepository<AppDbContext, DateTimeOffset?>(DbContext);
        }

        [Fact]
        public async Task Handler_UpdatesDateField_Success()
        {
            var deal = DbContext.Deals.First();
            var newDate = DateTimeOffset.UtcNow;
            var command = new UpdateDateFieldCommand
            {
                Entity = "Deal",
                Field = "CreatedAt",
                Id = deal.Id,
                Value = newDate,
                NotNull = true
            };

            var handler = new UpdateDateFieldCommand.Handler(_repo);
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Equal(newDate, result);
            var updated = DbContext.Deals.Find(deal.Id);
            Assert.Equal(newDate, updated.CreatedAt);
        }

        [Fact]
        public async Task Handler_TableNotExists_Throws()
        {
            var command = new UpdateDateFieldCommand
            {
                Entity = "NonExistingEntity",
                Field = "CreatedAt",
                Id = 1,
                Value = DateTimeOffset.UtcNow,
                NotNull = false
            };

            var handler = new UpdateDateFieldCommand.Handler(_repo);
            await Assert.ThrowsAsync<TableNotExistsException>(
                () => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handler_ColumnNotExists_Throws()
        {
            var deal = DbContext.Deals.First();
            var command = new UpdateDateFieldCommand
            {
                Entity = "Deal",
                Field = "NonExistingField",
                Id = deal.Id,
                Value = DateTimeOffset.UtcNow,
                NotNull = false
            };

            var handler = new UpdateDateFieldCommand.Handler(_repo);
            await Assert.ThrowsAsync<ColumnNotExistsException>(
                () => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handler_EntityNotFound_ThrowsNotFoundExceptionOverride()
        {
            var command = new UpdateDateFieldCommand
            {
                Entity = "Deal",
                Field = "CreatedAt",
                Id = -9999,
                Value = DateTimeOffset.UtcNow,
                NotNull = false
            };

            var handler = new UpdateDateFieldCommand.Handler(_repo);
            await Assert.ThrowsAsync<NotFoundExceptionOverride>(
                () => handler.Handle(command, CancellationToken.None));
        }
    }
}