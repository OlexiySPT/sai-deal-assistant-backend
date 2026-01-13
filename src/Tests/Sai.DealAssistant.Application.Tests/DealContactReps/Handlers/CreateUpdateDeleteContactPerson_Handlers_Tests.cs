using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Sai.DealAssistant.Application.Common.Exceptions;
using Xunit;
using Sai.DealAssistant.Application.Entities.ContactPersons.Commands;
using Sai.DealAssistant.Application.Entities.ContactPersons.Dto;
using Sai.DealAssistant.Application.Entities.DealTags.Commands;

namespace Sai.DealAssistant.Application.Tests.DealContactReps.Handlers
{
    public class CreateUpdateDeleteContactPerson_Handlers_Tests
    {
        [Fact]
        public async Task Create_Handler_ReturnsDto_OnSuccess()
        {
            var repoMock = new Mock<ICrudRepository<ContactPerson>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new CreateContactPersonCommand
            {
                DealId = 1,
                Name = "New Rep",
                Email = "new@example.com"
            };

            var mappedEntity = new ContactPerson { Name = cmd.Name, Email = cmd.Email, DealId = cmd.DealId };
            var createdEntity = new ContactPerson { Id = 123, Name = cmd.Name, Email = cmd.Email, DealId = cmd.DealId };
            var returnedDto = new ContactPersonDto { Id = createdEntity.Id, Name = createdEntity.Name, Email = createdEntity.Email };

            mapperMock.Setup(m => m.Map<ContactPerson>(It.IsAny<CreateContactPersonCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.CreateAsync(mappedEntity)).ReturnsAsync(createdEntity);
            mapperMock.Setup(m => m.Map<ContactPersonDto>(createdEntity)).Returns(returnedDto);

            var handler = new CreateContactPersonCommand.Handler(repoMock.Object, mapperMock.Object);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.Equal(returnedDto.Id, result.Id);
            Assert.Equal(returnedDto.Name, result.Name);
            repoMock.Verify(r => r.CreateAsync(mappedEntity), Times.Once);
        }

        [Fact]
        public async Task Create_Handler_ThrowsNotFound_WhenCreateReturnsNull()
        {
            var repoMock = new Mock<ICrudRepository<ContactPerson>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new CreateContactPersonCommand { DealId = 1, Name = "X" };
            var mappedEntity = new ContactPerson { Name = "X", DealId = 1 };

            mapperMock.Setup(m => m.Map<ContactPerson>(It.IsAny<CreateContactPersonCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.CreateAsync(mappedEntity)).ReturnsAsync((ContactPerson?)null);

            var handler = new CreateContactPersonCommand.Handler(repoMock.Object, mapperMock.Object);

            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(cmd, CancellationToken.None));
        }

        [Fact]
        public async Task Update_Handler_ReturnsDto_OnSuccess()
        {
            var repoMock = new Mock<ICrudRepository<ContactPerson>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new UpdateContactPersonCommand { Id = 5, Name = "Updated", Email = "u@example.com" };
            var mappedEntity = new ContactPerson { Id = cmd.Id, Name = cmd.Name, Email = cmd.Email };
            var updatedEntity = new ContactPerson { Id = cmd.Id, Name = cmd.Name, Email = cmd.Email };
            var returnedDto = new ContactPersonDto { Id = cmd.Id, Name = cmd.Name, Email = cmd.Email };

            mapperMock.Setup(m => m.Map<ContactPerson>(It.IsAny<UpdateContactPersonCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.UpdateAsync(mappedEntity)).ReturnsAsync(updatedEntity);
            mapperMock.Setup(m => m.Map<ContactPersonDto>(updatedEntity)).Returns(returnedDto);

            var handler = new UpdateContactPersonCommand.Handler(repoMock.Object, mapperMock.Object);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.Equal(returnedDto.Id, result.Id);
            Assert.Equal(returnedDto.Name, result.Name);
            repoMock.Verify(r => r.UpdateAsync(mappedEntity), Times.Once);
        }

        [Fact]
        public async Task Update_Handler_ThrowsNotFound_WhenUpdateReturnsNull()
        {
            var repoMock = new Mock<ICrudRepository<ContactPerson>>();
            var mapperMock = new Mock<IMapper>();

            var cmd = new UpdateContactPersonCommand { Id = 99, Name = "X" };
            var mappedEntity = new ContactPerson { Id = cmd.Id, Name = cmd.Name };

            mapperMock.Setup(m => m.Map<ContactPerson>(It.IsAny<UpdateContactPersonCommand>())).Returns(mappedEntity);
            repoMock.Setup(r => r.UpdateAsync(mappedEntity)).ReturnsAsync((ContactPerson?)null);

            var handler = new UpdateContactPersonCommand.Handler(repoMock.Object, mapperMock.Object);

            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(cmd, CancellationToken.None));
        }

        [Fact]
        public async Task Delete_Handler_ReturnsDto_OnSuccess()
        {
            var repoMock = new Mock<ICrudRepository<ContactPerson>>();
            var mapperMock = new Mock<IMapper>();

            var deletedEntity = new ContactPerson { Id = 77, Name = "Del", Email = "d@example.com" };
            var returnedDto = new ContactPersonDto { Id = 77, Name = "Del", Email = "d@example.com" };

            repoMock.Setup(r => r.DeleteAsync(deletedEntity.Id)).ReturnsAsync(deletedEntity);
            mapperMock.Setup(m => m.Map<ContactPersonDto>(deletedEntity)).Returns(returnedDto);

            var handler = new DeleteContactPersonCommand.Handler(repoMock.Object, mapperMock.Object);

            var result = await handler.Handle(new DeleteContactPersonCommand(deletedEntity.Id), CancellationToken.None);

            Assert.Equal(returnedDto.Id, result.Id);
            repoMock.Verify(r => r.DeleteAsync(deletedEntity.Id), Times.Once);
        }

        [Fact]
        public async Task Delete_Handler_ThrowsNotFound_WhenDeleteReturnsNull()
        {
            var repoMock = new Mock<ICrudRepository<ContactPerson>>();
            var mapperMock = new Mock<IMapper>();

            repoMock.Setup(r => r.DeleteAsync(999)).ReturnsAsync((ContactPerson?)null);

            var handler = new DeleteContactPersonCommand.Handler(repoMock.Object, mapperMock.Object);

            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(new DeleteContactPersonCommand(999), CancellationToken.None));
        }
    }
}