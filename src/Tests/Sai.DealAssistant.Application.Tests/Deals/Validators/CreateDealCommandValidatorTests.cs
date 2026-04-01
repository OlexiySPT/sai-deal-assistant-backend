using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using Sai.DealAssistant.Application.Entities.Deals.Commands;
using Sai.DealAssistant.Domain.Entities.ReadOnly;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Deals.Validators;

public class CreateDealCommandValidatorTests
{
    [Fact]
    public async Task Validator_WithValidIds_PassesValidation()
    {
        // Arrange
        var dealStateCacheMock = new Mock<IEnumCache<DealState>>();
        var dealTypeCacheMock = new Mock<IEnumCache<DealType>>();

        dealStateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<DealState> { new DealState { Id = 1 } });
        dealTypeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<DealType> { new DealType { Id = 1 } });

        var validator = new CreateDealCommand.Validator(dealStateCacheMock.Object, dealTypeCacheMock.Object);

        var command = new CreateDealCommand
        {
            Name = "Valid",
            TypeId = 1,
            StateId = 1,
            FirmId = 1,
            Url = "https://example.com"
        };

        // Act
        var result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Name);
        result.ShouldNotHaveValidationErrorFor(c => c.TypeId);
        result.ShouldNotHaveValidationErrorFor(c => c.StateId);
        result.ShouldNotHaveValidationErrorFor(c => c.FirmId);
        result.ShouldNotHaveValidationErrorFor(c => c.Url);
    }

    [Fact]
    public async Task Validator_WithInvalidTypeId_FailsValidation()
    {
        var dealStateCacheMock = new Mock<IEnumCache<DealState>>();
        var dealTypeCacheMock = new Mock<IEnumCache<DealType>>();

        dealStateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<DealState>()); // no matching type id
        dealTypeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<DealType> { new DealType { Id = 1 } });

        var validator = new CreateDealCommand.Validator(dealStateCacheMock.Object, dealTypeCacheMock.Object);

        var command = new CreateDealCommand { Name = "X", TypeId = 99, StateId = 1 };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.TypeId);
    }

    [Fact]
    public async Task Validator_WithInvalidStateId_FailsValidation()
    {
        var dealStateCacheMock = new Mock<IEnumCache<DealState>>();
        var dealTypeCacheMock = new Mock<IEnumCache<DealType>>();

        dealStateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<DealState> { new DealState { Id = 1 } });
        dealTypeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<DealType>()); // no matching state id

        var validator = new CreateDealCommand.Validator(dealStateCacheMock.Object, dealTypeCacheMock.Object);

        var command = new CreateDealCommand { Name = "X", TypeId = 1, StateId = 99 };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.StateId);
    }

    [Fact]
    public async Task Validator_InvalidUrl_FailsValidation()
    {
        var dealStateCacheMock = new Mock<IEnumCache<DealState>>();
        var dealTypeCacheMock = new Mock<IEnumCache<DealType>>();

        dealStateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<DealState> { new DealState { Id = 1 } });
        dealTypeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<DealType> { new DealType { Id = 1 } });

        var validator = new CreateDealCommand.Validator(dealStateCacheMock.Object, dealTypeCacheMock.Object);

        var command = new CreateDealCommand { Name = "X", TypeId = 1, StateId = 1, FirmId = 1, Url = "not-a-url" };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Url);
    }

    [Fact]
    public async Task Validator_WithMissingFirmId_FailsValidation()
    {
        var dealStateCacheMock = new Mock<IEnumCache<DealState>>();
        var dealTypeCacheMock = new Mock<IEnumCache<DealType>>();

        dealStateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<DealState> { new DealState { Id = 1 } });
        dealTypeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<DealType> { new DealType { Id = 1 } });

        var validator = new CreateDealCommand.Validator(dealStateCacheMock.Object, dealTypeCacheMock.Object);

        var command = new CreateDealCommand { Name = "X", TypeId = 1, StateId = 1, FirmId = 0 };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.FirmId);
    }
}