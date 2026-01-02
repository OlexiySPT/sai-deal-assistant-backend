using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Sai.DealAssistant.Application.Entities.Events.Commands;
using Sai.DealAssistant.Application.Entities.Events.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Sai.DealAssistant.Application.Common.Exceptions;
using Xunit;
using System;

namespace Sai.DealAssistant.Application.Tests.Events.Handlers
{
    public class CreateUpdateDeleteEvent_Handlers_Tests
    {
        [Fact]
        public async Task Create_Handler_ReturnsDto_OnSuccess()
        {
            var repoMock = new Mock<ICrudRepository<Event>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new CreateEventCommand
            {
                DealId = 1,
                Date = DateTimeOffset.UtcNow,
                TypeId = 1,
                StateId = 1
            };

            var mappedEntity = new Event { Date = cmd.Date, DealId = cmd.DealId, TypeId = cmd.TypeId, StateId = cmd.StateId };
            var createdEntity = new Event { Id = 123, Date = cmd.Date, DealId = cmd.DealId, TypeId = cmd.TypeId, StateId = cmd.StateId };
            var returnedDto = new EventDto { Id = createdEntity.Id, Date = createdEntity.Date, TypeId = createdEntity.TypeId, StateId = createdEntity.StateId };

            mapperMock.Setup(m => m.Map<Event>(It.IsAny<CreateEventCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.CreateAsync(mappedEntity)).ReturnsAsync(createdEntity);
            mapperMock.Setup(m => m.Map<EventDto>(createdEntity)).Returns(returnedDto);

            var handler = new CreateEventCommand.Handler(repoMock.Object, mapperMock.Object);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.Equal(returnedDto.Id, result.Id);
            Assert.Equal(returnedDto.Date, result.Date);
            repoMock.Verify(r => r.CreateAsync(mappedEntity), Times.Once);
        }

        [Fact]
        public async Task Create_Handler_ThrowsNotFound_WhenCreateReturnsNull()
        {
            var repoMock = new Mock<ICrudRepository<Event>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new CreateEventCommand { DealId = 1, Date = DateTimeOffset.UtcNow };
            var mappedEntity = new Event { Date = cmd.Date, DealId = cmd.DealId };

            mapperMock.Setup(m => m.Map<Event>(It.IsAny<CreateEventCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.CreateAsync(mappedEntity)).ReturnsAsync((Event?)null);

            var handler = new CreateEventCommand.Handler(repoMock.Object, mapperMock.Object);

            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(cmd, CancellationToken.None));
        }

        [Fact]
        public async Task Update_Handler_ReturnsDto_OnSuccess()
        {
            var repoMock = new Mock<ICrudRepository<Event>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new UpdateEventCommand { Id = 5, Date = DateTimeOffset.UtcNow, TypeId = 1, StateId = 1 };
            var mappedEntity = new Event { Id = cmd.Id, Date = cmd.Date, TypeId = cmd.TypeId, StateId = cmd.StateId };
            var updatedEntity = new Event { Id = cmd.Id, Date = cmd.Date, TypeId = cmd.TypeId, StateId = cmd.StateId };
            var returnedDto = new EventDto { Id = cmd.Id, Date = cmd.Date, TypeId = cmd.TypeId, StateId = cmd.StateId };

            mapperMock.Setup(m => m.Map<Event>(It.IsAny<UpdateEventCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.UpdateAsync(mappedEntity)).ReturnsAsync(updatedEntity);
            mapperMock.Setup(m => m.Map<EventDto>(updatedEntity)).Returns(returnedDto);

            var handler = new UpdateEventCommand.Handler(repoMock.Object, mapperMock.Object);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.Equal(returnedDto.Id, result.Id);
            Assert.Equal(returnedDto.Date, result.Date);
            repoMock.Verify(r => r.UpdateAsync(mappedEntity), Times.Once);
        }

        [Fact]
        public async Task Update_Handler_ThrowsNotFound_WhenUpdateReturnsNull()
        {
            var repoMock = new Mock<ICrudRepository<Event>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new UpdateEventCommand { Id = 99, Date = DateTimeOffset.UtcNow };
            var mappedEntity = new Event { Id = cmd.Id, Date = cmd.Date };

            mapperMock.Setup(m => m.Map<Event>(It.IsAny<UpdateEventCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.UpdateAsync(mappedEntity)).ReturnsAsync((Event?)null);

            var handler = new UpdateEventCommand.Handler(repoMock.Object, mapperMock.Object);

            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(cmd, CancellationToken.None));
        }

        [Fact]
        public async Task Delete_Handler_ReturnsDto_OnSuccess()
        {
            var repoMock = new Mock<ICrudRepository<Event>>();
            var mapperMock = new Mock<IMapper>();

            var deletedEntity = new Event { Id = 77, Date = DateTimeOffset.UtcNow, Agenda = "Del" };
            var returnedDto = new EventDto { Id = 77, Date = deletedEntity.Date, Agenda = deletedEntity.Agenda };

            repoMock.Setup(r => r.DeleteAsync(deletedEntity.Id)).ReturnsAsync(deletedEntity);
            mapperMock.Setup(m => m.Map<EventDto>(deletedEntity)).Returns(returnedDto);

            var handler = new DeleteEventCommand.Handler(repoMock.Object, mapperMock.Object);

            var result = await handler.Handle(new DeleteEventCommand(deletedEntity.Id), CancellationToken.None);

            Assert.Equal(returnedDto.Id, result.Id);
            repoMock.Verify(r => r.DeleteAsync(deletedEntity.Id), Times.Once);
        }

        [Fact]
        public async Task Delete_Handler_ThrowsNotFound_WhenDeleteReturnsNull()
        {
            var repoMock = new Mock<ICrudRepository<Event>>();
            var mapperMock = new Mock<IMapper>();

            repoMock.Setup(r => r.DeleteAsync(999)).ReturnsAsync((Event?)null);

            var handler = new DeleteEventCommand.Handler(repoMock.Object, mapperMock.Object);

            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(new DeleteEventCommand(999), CancellationToken.None));
        }
    }
}