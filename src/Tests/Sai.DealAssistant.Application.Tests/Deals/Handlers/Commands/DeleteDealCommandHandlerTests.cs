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

public class DeleteDealCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenRepositoryDeletes_ReturnsMappedDto()
    {
        // Arrange
        var repositoryMock = new Mock<ICrudRepository<Deal>>();
        var mapperMock = new Mock<IMapper>();

        var deletedDeal = new Deal { Id = 7, Name = "ToDelete" };
        var mappedDto = new DealDto { Id = 7, Name = "ToDelete" };

        repositoryMock.Setup(r => r.DeleteAsync(7)).ReturnsAsync(deletedDeal);
        mapperMock.Setup(m => m.Map<DealDto>(deletedDeal)).Returns(mappedDto);

        var handler = new DeleteDealCommand.Handler(repositoryMock.Object, mapperMock.Object);

        // Act
        var result = await handler.Handle(new DeleteDealCommand(7), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(mappedDto);
        repositoryMock.Verify(r => r.DeleteAsync(7), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenDeleteReturnsNull_ThrowsNotFoundException()
    {
        // Arrange
        var repositoryMock = new Mock<ICrudRepository<Deal>>();
        var mapperMock = new Mock<IMapper>();

        repositoryMock.Setup(r => r.DeleteAsync(13)).ReturnsAsync((Deal)null);

        var handler = new DeleteDealCommand.Handler(repositoryMock.Object, mapperMock.Object);

        // Act / Assert
        await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(new DeleteDealCommand(13), CancellationToken.None));
    }
}