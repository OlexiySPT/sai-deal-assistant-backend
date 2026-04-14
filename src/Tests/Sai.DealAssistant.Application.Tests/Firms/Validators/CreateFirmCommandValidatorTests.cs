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

public class CreateFirmCommandValidatorTests
{
    private static Mock<IReadRepository<Firm>> CreateFirmRepoMock(bool firmExists)
    {
        var mock = new Mock<IReadRepository<Firm>>();
        mock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Firm, bool>>>()))
            .ReturnsAsync(firmExists);
        return mock;
    }

    [Fact]
    public async Task Validator_WithValidData_PassesValidation()
    {
        // Arrange
        var firmRepoMock = CreateFirmRepoMock(firmExists: false);
        var validator = new CreateFirmCommand.Validator(firmRepoMock.Object);
        var command = new CreateFirmCommand
        {
            Name = "New Firm",
            Country = "Germany"
        };

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Name);
        result.ShouldNotHaveValidationErrorFor(c => c.Country);
    }

    [Fact]
    public async Task Validator_WithEmptyName_FailsValidation()
    {
        // Arrange
        var firmRepoMock = CreateFirmRepoMock(firmExists: false);
        var validator = new CreateFirmCommand.Validator(firmRepoMock.Object);
        var command = new CreateFirmCommand { Name = "", Country = "Germany" };

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public async Task Validator_WithNameExceeding100Chars_FailsValidation()
    {
        // Arrange
        var firmRepoMock = CreateFirmRepoMock(firmExists: false);
        var validator = new CreateFirmCommand.Validator(firmRepoMock.Object);
        var command = new CreateFirmCommand
        {
            Name = new string('A', 101),
            Country = "Germany"
        };

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public async Task Validator_WithDuplicateName_FailsValidation()
    {
        // Arrange
        var firmRepoMock = CreateFirmRepoMock(firmExists: true);
        var validator = new CreateFirmCommand.Validator(firmRepoMock.Object);
        var command = new CreateFirmCommand
        {
            Name = "Existing Firm",
            Country = "Germany"
        };

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name)
            .WithErrorMessage("Firm with name 'Existing Firm' already exists.");
    }

    [Fact]
    public async Task Validator_WithEmptyCountry_FailsValidation()
    {
        // Arrange
        var firmRepoMock = CreateFirmRepoMock(firmExists: false);
        var validator = new CreateFirmCommand.Validator(firmRepoMock.Object);
        var command = new CreateFirmCommand { Name = "New Firm", Country = "" };

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Country);
    }

    [Fact]
    public async Task Validator_WithCountryExceeding100Chars_FailsValidation()
    {
        // Arrange
        var firmRepoMock = CreateFirmRepoMock(firmExists: false);
        var validator = new CreateFirmCommand.Validator(firmRepoMock.Object);
        var command = new CreateFirmCommand
        {
            Name = "New Firm",
            Country = new string('A', 101)
        };

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Country);
    }
}
