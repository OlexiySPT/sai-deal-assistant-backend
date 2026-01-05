using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Sai.DealAssistant.Application.Entities.Deals.Commands;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Deals.Handlers.Commands;

public class UpdateDealCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenRepositoryUpdates_ReturnsMappedDto()
    {
        // Arrange
        var repositoryMock = new Mock<ICrudRepository<Deal>>();
        var mapperMock = new Mock<IMapper>();

        var command = new UpdateDealCommand
        {
            Id = 5,
            Name = "Updated",
            TypeId = 1,
            StateId = 1
        };

        var mappedDeal = new Deal { Id = 5, Name = "Updated" };
        var updatedDeal = new Deal { Id = 5, Name = "Updated" };
        var mappedDto = new DealDto { Id = 5, Name = "Updated" };

        mapperMock.Setup(m => m.Map<Deal>(It.IsAny<UpdateDealCommand>())).Returns(mappedDeal);
        repositoryMock.Setup(r => r.UpdateAsync(mappedDeal)).ReturnsAsync(updatedDeal);
        mapperMock.Setup(m => m.Map<DealDto>(updatedDeal)).Returns(mappedDto);

        var handler = new UpdateDealCommand.Handler(repositoryMock.Object, mapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(mappedDto);
        repositoryMock.Verify(r => r.UpdateAsync(mappedDeal), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsNull_ThrowsNotFoundException()
    {
        // Arrange
        var repositoryMock = new Mock<ICrudRepository<Deal>>();
        var mapperMock = new Mock<IMapper>();

        var command = new UpdateDealCommand { Id = 99, Name = "DoesNotExist", TypeId = 1, StateId = 1 };
        var mappedDeal = new Deal { Id = 99, Name = "DoesNotExist" };

        mapperMock.Setup(m => m.Map<Deal>(It.IsAny<UpdateDealCommand>())).Returns(mappedDeal);
        repositoryMock.Setup(r => r.UpdateAsync(mappedDeal)).ReturnsAsync((Deal)null);

        var handler = new UpdateDealCommand.Handler(repositoryMock.Object, mapperMock.Object);

        // Act / Assert
        await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(command, CancellationToken.None));
    }
}