using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using Sai.DealAssistant.Application.Entities.Firms.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Firms.Validators;

public class DeleteFirmCommandValidatorTests
{
    [Fact]
    public async Task Validator_WithNoAssociatedDeals_PassesValidation()
    {
        // Arrange
        var dealRepoMock = new Mock<IReadRepository<Deal>>();
        dealRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Deal, bool>>>()))
            .ReturnsAsync(false);

        var validator = new DeleteFirmCommand.Validator(dealRepoMock.Object);
        var command = new DeleteFirmCommand(1);

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Id);
    }

    [Fact]
    public async Task Validator_WithAssociatedDeals_FailsValidation()
    {
        // Arrange
        var dealRepoMock = new Mock<IReadRepository<Deal>>();
        dealRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Deal, bool>>>()))
            .ReturnsAsync(true);

        var validator = new DeleteFirmCommand.Validator(dealRepoMock.Object);
        var command = new DeleteFirmCommand(5);

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Id)
            .WithErrorMessage("Firm with Id 5 cannot be deleted because it has associated Deals.");
    }
}
