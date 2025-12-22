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

namespace Sai.DealAssistant.Application.Tests.Deals;

public class CreateDealCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenRepositoryCreates_ReturnsMappedDto()
    {
        // Arrange
        var repositoryMock = new Mock<ICrudRepository<Deal>>();
        var mapperMock = new Mock<IMapper>();

        var command = new CreateDealCommand
        {
            Name = "Test Deal",
            TypeId = 1,
            StateId = 1
        };

        var mappedDeal = new Deal { Id = 42, Name = "Test Deal" };
        var mappedDto = new DealDto { Id = 42, Name = "Test Deal" };

        mapperMock.Setup(m => m.Map<Deal>(It.IsAny<CreateDealCommand>())).Returns(mappedDeal);
        repositoryMock.Setup(r => r.CreateAsync(mappedDeal)).ReturnsAsync(mappedDeal);
        mapperMock.Setup(m => m.Map<DealDto>(mappedDeal)).Returns(mappedDto);

        var handler = new CreateDealCommand.Handler(repositoryMock.Object, mapperMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(mappedDto);
        repositoryMock.Verify(r => r.CreateAsync(mappedDeal), Times.Once);
        mapperMock.Verify(m => m.Map<Deal>(It.IsAny<CreateDealCommand>()), Times.Once);
        mapperMock.Verify(m => m.Map<DealDto>(mappedDeal), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsNull_ThrowsNotFoundException()
    {
        // Arrange
        var repositoryMock = new Mock<ICrudRepository<Deal>>();
        var mapperMock = new Mock<IMapper>();

        var command = new CreateDealCommand { Name = "Missing Deal", TypeId = 1, StateId = 1 };
        var mappedDeal = new Deal { Name = "Missing Deal" };

        mapperMock.Setup(m => m.Map<Deal>(It.IsAny<CreateDealCommand>())).Returns(mappedDeal);
        repositoryMock.Setup(r => r.CreateAsync(mappedDeal)).ReturnsAsync((Deal?)null);

        var handler = new CreateDealCommand.Handler(repositoryMock.Object, mapperMock.Object);

        // Act / Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}