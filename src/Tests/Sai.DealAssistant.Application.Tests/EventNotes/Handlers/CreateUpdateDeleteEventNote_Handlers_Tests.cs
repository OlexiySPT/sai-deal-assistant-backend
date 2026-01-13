using AutoMapper;
using Moq;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.EventNotes.Commands;
using Sai.DealAssistant.Application.Entities.EventNotes.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Sai.DealAssistant.Application.Tests.EventNotes.Handlers
{
    public class CreateUpdateDeleteEventNote_Handlers_Tests
    {
        [Fact]
        public async Task Create_Handler_ReturnsDto_OnSuccess()
        {
            var repoMock = new Mock<ICrudRepository<EventNote>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new CreateEventNoteCommand
            {
                EventId = 1,
                Text = "New note",
                Order = 1
            };

            var mappedEntity = new EventNote { Text = cmd.Text, Order = cmd.Order, EventId = cmd.EventId };
            var createdEntity = new EventNote { Id = 123, Text = cmd.Text, Order = cmd.Order, EventId = cmd.EventId };
            var returnedDto = new EventNoteDto { Id = createdEntity.Id, Text = createdEntity.Text, Order = createdEntity.Order, EventId = createdEntity.EventId };

            mapperMock.Setup(m => m.Map<EventNote>(It.IsAny<CreateEventNoteCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.CreateAsync(mappedEntity)).ReturnsAsync(createdEntity);
            mapperMock.Setup(m => m.Map<EventNoteDto>(createdEntity)).Returns(returnedDto);

            var handler = new CreateEventNoteCommand.Handler(repoMock.Object, mapperMock.Object);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.Equal(returnedDto.Id, result.Id);
            Assert.Equal(returnedDto.Text, result.Text);
            repoMock.Verify(r => r.CreateAsync(mappedEntity), Times.Once);
        }

        [Fact]
        public async Task Create_Handler_ThrowsNotFound_WhenCreateReturnsNull()
        {
            var repoMock = new Mock<ICrudRepository<EventNote>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new CreateEventNoteCommand { EventId = 1, Text = "X" };
            var mappedEntity = new EventNote { Text = "X", EventId = 1 };

            mapperMock.Setup(m => m.Map<EventNote>(It.IsAny<CreateEventNoteCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.CreateAsync(mappedEntity)).ReturnsAsync((EventNote?)null);

            var handler = new CreateEventNoteCommand.Handler(repoMock.Object, mapperMock.Object);

            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(cmd, CancellationToken.None));
        }

        [Fact]
        public async Task Update_Handler_ReturnsDto_OnSuccess()
        {
            var repoMock = new Mock<ICrudRepository<EventNote>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new UpdateEventNoteCommand { Id = 5, Text = "Updated note", Order = 2, EventId = 1 };
            var mappedEntity = new EventNote { Id = cmd.Id, Text = cmd.Text, Order = cmd.Order, EventId = cmd.EventId };
            var updatedEntity = new EventNote { Id = cmd.Id, Text = cmd.Text, Order = cmd.Order, EventId = cmd.EventId };
            var returnedDto = new EventNoteDto { Id = cmd.Id, Text = cmd.Text, Order = cmd.Order, EventId = cmd.EventId };

            mapperMock.Setup(m => m.Map<EventNote>(It.IsAny<UpdateEventNoteCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.UpdateAsync(mappedEntity)).ReturnsAsync(updatedEntity);
            mapperMock.Setup(m => m.Map<EventNoteDto>(updatedEntity)).Returns(returnedDto);

            var handler = new UpdateEventNoteCommand.Handler(repoMock.Object, mapperMock.Object);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.Equal(returnedDto.Id, result.Id);
            Assert.Equal(returnedDto.Text, result.Text);
            repoMock.Verify(r => r.UpdateAsync(mappedEntity), Times.Once);
        }

        [Fact]
        public async Task Update_Handler_ThrowsNotFound_WhenUpdateReturnsNull()
        {
            var repoMock = new Mock<ICrudRepository<EventNote>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new UpdateEventNoteCommand { Id = 99, Text = "X", EventId = 1 };
            var mappedEntity = new EventNote { Id = cmd.Id, Text = cmd.Text, EventId = cmd.EventId };

            mapperMock.Setup(m => m.Map<EventNote>(It.IsAny<UpdateEventNoteCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.UpdateAsync(mappedEntity)).ReturnsAsync((EventNote?)null);

            var handler = new UpdateEventNoteCommand.Handler(repoMock.Object, mapperMock.Object);

            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(cmd, CancellationToken.None));
        }

        [Fact]
        public async Task Delete_Handler_ReturnsDto_OnSuccess()
        {
            var repoMock = new Mock<ICrudRepository<EventNote>>();
            var mapperMock = new Mock<IMapper>();

            var deletedEntity = new EventNote { Id = 77, Text = "Del note", Order = 1, EventId = 5 };
            var returnedDto = new EventNoteDto { Id = 77, Text = "Del note", Order = 1, EventId = 5 };

            repoMock.Setup(r => r.DeleteAsync(deletedEntity.Id)).ReturnsAsync(deletedEntity);
            mapperMock.Setup(m => m.Map<EventNoteDto>(deletedEntity)).Returns(returnedDto);

            var handler = new DeleteEventNoteCommand.Handler(repoMock.Object, mapperMock.Object);

            var result = await handler.Handle(new DeleteEventNoteCommand(deletedEntity.Id), CancellationToken.None);

            Assert.Equal(returnedDto.Id, result.Id);
            Assert.Equal(returnedDto.Text, result.Text);
            repoMock.Verify(r => r.DeleteAsync(deletedEntity.Id), Times.Once);
        }

        [Fact]
        public async Task Delete_Handler_ThrowsNotFound_WhenDeleteReturnsNull()
        {
            var repoMock = new Mock<ICrudRepository<EventNote>>();
            var mapperMock = new Mock<IMapper>();

            repoMock.Setup(r => r.DeleteAsync(999)).ReturnsAsync((EventNote?)null);

            var handler = new DeleteEventNoteCommand.Handler(repoMock.Object, mapperMock.Object);

            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(new DeleteEventNoteCommand(999), CancellationToken.None));
        }
    }
}