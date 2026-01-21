using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.DealTags.Commands;
using Sai.DealAssistant.Application.Entities.DealTags.Dto;
using Sai.DealAssistant.Application.Entities.ContactPersons.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.DealTags.Handlers
{
    public class AddDeleteDealTag_Handlers_Tests : UnitTestBase
    {
        private readonly CrudRepository<AppDbContext, DealTag> _crudRepo;
        private readonly IMapper _mapper;

        public AddDeleteDealTag_Handlers_Tests()
            : base(seedTestData: true)
        {
            _crudRepo = new CrudRepository<AppDbContext, DealTag>(DbContext);

            var cfg = new MapperConfiguration(cfg =>
            {
                // mappings required by handlers/tests
                cfg.CreateMap<AddDealTagIfNotExistsCommand, DealTag>();
                cfg.CreateMap<DealTag, DealTagDto>();
            },
            LoggerFactory);

            _mapper = cfg.CreateMapper();
        }

        [Fact]
        public async Task AddDealTagIfNotExists_ReturnsExisting_WhenTagAlreadyPresent_IgnoringCase()
        {
            // Arrange
            var existing = await DbContext.Set<DealTag>().AsNoTracking().FirstAsync();
            var handler = new AddDealTagIfNotExistsCommand.Handler(_crudRepo, _mapper);

            var cmd = new AddDealTagIfNotExistsCommand
            {
                DealId = existing.DealId,
                Tag = existing.Tag.ToUpperInvariant() // different case to ensure case-insensitive match
            };

            var beforeCount = await DbContext.Set<DealTag>().Where(t => t.DealId == existing.DealId).CountAsync();

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existing.Id, result.Id);
            var afterCount = await DbContext.Set<DealTag>().Where(t => t.DealId == existing.DealId).CountAsync();
            Assert.Equal(beforeCount, afterCount); // no duplicate created
        }

        [Fact]
        public async Task AddDealTagIfNotExists_CreatesNewTag_WhenNotExists()
        {
            // Arrange
            var dealId = await DbContext.Set<Domain.Entities.Deal>().Select(d => d.Id).FirstAsync();
            var uniqueTag = "utest_" + Guid.NewGuid().ToString("N");
            var handler = new AddDealTagIfNotExistsCommand.Handler(_crudRepo, _mapper);

            var cmd = new AddDealTagIfNotExistsCommand
            {
                DealId = dealId,
                Tag = uniqueTag
            };

            // Act
            var result = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal(dealId, result.DealId);
            Assert.Equal(uniqueTag, result.Tag);

            var existsInDb = await DbContext.Set<DealTag>().AnyAsync(t => t.Id == result.Id && t.Tag == uniqueTag && t.DealId == dealId);
            Assert.True(existsInDb);
        }

        [Fact]
        public async Task DeleteDealTagCommand_ReturnsDeletedDto_WhenExists()
        {
            // Arrange
            var existing = await DbContext.Set<DealTag>().FirstAsync();
            var handler = new DeleteDealTagCommand.Handler(_crudRepo, _mapper);

            // Act
            var result = await handler.Handle(new DeleteDealTagCommand(existing.Id), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existing.Id, result.Id);

            var stillExists = await DbContext.Set<DealTag>().AnyAsync(t => t.Id == existing.Id);
            Assert.False(stillExists);
        }

        [Fact]
        public async Task DeleteDealTagCommand_ThrowsNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            var handler = new DeleteDealTagCommand.Handler(_crudRepo, _mapper);
            var nonExistingId = -9999;

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() =>
                handler.Handle(new DeleteDealTagCommand(nonExistingId), CancellationToken.None));
        }
    }
}