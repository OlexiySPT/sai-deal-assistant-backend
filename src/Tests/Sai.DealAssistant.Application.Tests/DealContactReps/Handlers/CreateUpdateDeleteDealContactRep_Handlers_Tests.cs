using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Sai.DealAssistant.Application.Entities.DealContactReps.Commands;
using Sai.DealAssistant.Application.Entities.DealContactReps.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Sai.DealAssistant.Application.Common.Exceptions;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.DealContactReps.Handlers
{
    public class CreateUpdateDeleteDealContactRep_Handlers_Tests
    {
        [Fact]
        public async Task Create_Handler_ReturnsDto_OnSuccess()
        {
            var repoMock = new Mock<ICrudRepository<DealContactRep>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new CreateDealContactRepCommand
            {
                DealId = 1,
                Name = "New Rep",
                Email = "new@example.com"
            };

            var mappedEntity = new DealContactRep { Name = cmd.Name, Email = cmd.Email, DealId = cmd.DealId };
            var createdEntity = new DealContactRep { Id = 123, Name = cmd.Name, Email = cmd.Email, DealId = cmd.DealId };
            var returnedDto = new DealContactRepDto { Id = createdEntity.Id, Name = createdEntity.Name, Email = createdEntity.Email };

            mapperMock.Setup(m => m.Map<DealContactRep>(It.IsAny<CreateDealContactRepCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.CreateAsync(mappedEntity)).ReturnsAsync(createdEntity);
            mapperMock.Setup(m => m.Map<DealContactRepDto>(createdEntity)).Returns(returnedDto);

            var handler = new CreateDealContactRepCommand.Handler(repoMock.Object, mapperMock.Object);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.Equal(returnedDto.Id, result.Id);
            Assert.Equal(returnedDto.Name, result.Name);
            repoMock.Verify(r => r.CreateAsync(mappedEntity), Times.Once);
        }

        [Fact]
        public async Task Create_Handler_ThrowsNotFound_WhenCreateReturnsNull()
        {
            var repoMock = new Mock<ICrudRepository<DealContactRep>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new CreateDealContactRepCommand { DealId = 1, Name = "X" };
            var mappedEntity = new DealContactRep { Name = "X", DealId = 1 };

            mapperMock.Setup(m => m.Map<DealContactRep>(It.IsAny<CreateDealContactRepCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.CreateAsync(mappedEntity)).ReturnsAsync((DealContactRep?)null);

            var handler = new CreateDealContactRepCommand.Handler(repoMock.Object, mapperMock.Object);

            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(cmd, CancellationToken.None));
        }

        [Fact]
        public async Task Update_Handler_ReturnsDto_OnSuccess()
        {
            var repoMock = new Mock<ICrudRepository<DealContactRep>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new UpdateDealContactRepCommand { Id = 5, Name = "Updated", Email = "u@example.com" };
            var mappedEntity = new DealContactRep { Id = cmd.Id, Name = cmd.Name, Email = cmd.Email };
            var updatedEntity = new DealContactRep { Id = cmd.Id, Name = cmd.Name, Email = cmd.Email };
            var returnedDto = new DealContactRepDto { Id = cmd.Id, Name = cmd.Name, Email = cmd.Email };

            mapperMock.Setup(m => m.Map<DealContactRep>(It.IsAny<UpdateDealContactRepCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.UpdateAsync(mappedEntity)).ReturnsAsync(updatedEntity);
            mapperMock.Setup(m => m.Map<DealContactRepDto>(updatedEntity)).Returns(returnedDto);

            var handler = new UpdateDealContactRepCommand.Handler(repoMock.Object, mapperMock.Object);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.Equal(returnedDto.Id, result.Id);
            Assert.Equal(returnedDto.Name, result.Name);
            repoMock.Verify(r => r.UpdateAsync(mappedEntity), Times.Once);
        }

        [Fact]
        public async Task Update_Handler_ThrowsNotFound_WhenUpdateReturnsNull()
        {
            var repoMock = new Mock<ICrudRepository<DealContactRep>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new UpdateDealContactRepCommand { Id = 99, Name = "X" };
            var mappedEntity = new DealContactRep { Id = cmd.Id, Name = cmd.Name };

            mapperMock.Setup(m => m.Map<DealContactRep>(It.IsAny<UpdateDealContactRepCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.UpdateAsync(mappedEntity)).ReturnsAsync((DealContactRep?)null);

            var handler = new UpdateDealContactRepCommand.Handler(repoMock.Object, mapperMock.Object);

            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(cmd, CancellationToken.None));
        }

        [Fact]
        public async Task Delete_Handler_ReturnsDto_OnSuccess()
        {
            var repoMock = new Mock<ICrudRepository<DealContactRep>>();
            var mapperMock = new Mock<IMapper>();

            var deletedEntity = new DealContactRep { Id = 77, Name = "Del", Email = "d@example.com" };
            var returnedDto = new DealContactRepDto { Id = 77, Name = "Del", Email = "d@example.com" };

            repoMock.Setup(r => r.DeleteAsync(deletedEntity.Id)).ReturnsAsync(deletedEntity);
            mapperMock.Setup(m => m.Map<DealContactRepDto>(deletedEntity)).Returns(returnedDto);

            var handler = new DeleteDealContactRepCommand.Handler(repoMock.Object, mapperMock.Object);

            var result = await handler.Handle(new DeleteDealContactRepCommand(deletedEntity.Id), CancellationToken.None);

            Assert.Equal(returnedDto.Id, result.Id);
            repoMock.Verify(r => r.DeleteAsync(deletedEntity.Id), Times.Once);
        }

        [Fact]
        public async Task Delete_Handler_ThrowsNotFound_WhenDeleteReturnsNull()
        {
            var repoMock = new Mock<ICrudRepository<DealContactRep>>();
            var mapperMock = new Mock<IMapper>();

            repoMock.Setup(r => r.DeleteAsync(999)).ReturnsAsync((DealContactRep?)null);

            var handler = new DeleteDealContactRepCommand.Handler(repoMock.Object, mapperMock.Object);

            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(new DeleteDealContactRepCommand(999), CancellationToken.None));
        }
    }
}