using FluentAssertions;
using Sai.DealAssistant.Application.Entities.Products.Commands;
using Sai.DealAssistant.Application.Entities.Products.Handlers;
using Sai.DealAssistant.Infrastructure.Repositories;
using SAI.DealAssistant.TestUtils.Unit;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Application
{
    public class CreateProductHandlerTests : DealAsistantUnitTestBase
    {
        private readonly ProductRepository _repository;
        private readonly CreateProductHandler _handler;

        public CreateProductHandlerTests()
            :base(true)
        {
            _repository = new ProductRepository(DbContext);
            _handler = new CreateProductHandler(_repository);
        }

        [Fact]
        public async Task Handle_Should_Create_Product_And_Return_Id()
        {
            // Arrange
            var command = new CreateProductCommand("Test Product", 99.99m);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
            var product = await DbContext.Products.FindAsync(result);
            product.Should().NotBeNull();
            product!.Name.Should().Be("Test Product");
            product.Price.Should().Be(99.99m);
        }
    }
}