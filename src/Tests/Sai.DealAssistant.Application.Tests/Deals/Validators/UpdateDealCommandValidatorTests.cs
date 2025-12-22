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

public class UpdateDealCommandValidatorTests
{
    [Fact]
    public async Task Validator_WithValidIds_PassesValidation()
    {
        var dealStateCacheMock = new Mock<IEnumCache<DealState>>();
        var dealTypeCacheMock = new Mock<IEnumCache<DealType>>();

        dealStateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<DealState> { new DealState { Id = 1 } });
        dealTypeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<DealType> { new DealType { Id = 1 } });

        var validator = new UpdateDealCommand.Validator(dealStateCacheMock.Object, dealTypeCacheMock.Object);

        // Update validator in source inherits AbstractValidator<CreateDealCommand>,
        // so validate a CreateDealCommand instance here to match source behavior.
        var command = new CreateDealCommand { Name = "OK", TypeId = 1, StateId = 1, Url = "https://ex.com" };

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Name);
        result.ShouldNotHaveValidationErrorFor(c => c.TypeId);
        result.ShouldNotHaveValidationErrorFor(c => c.StateId);
        result.ShouldNotHaveValidationErrorFor(c => c.Url);
    }

    [Fact]
    public async Task Validator_InvalidStateId_FailsValidation()
    {
        var dealStateCacheMock = new Mock<IEnumCache<DealState>>();
        var dealTypeCacheMock = new Mock<IEnumCache<DealType>>();

        dealStateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<DealState> { new DealState { Id = 1 } });
        dealTypeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<DealType>()); // no states

        var validator = new UpdateDealCommand.Validator(dealStateCacheMock.Object, dealTypeCacheMock.Object);
        var command = new CreateDealCommand { Name = "OK", TypeId = 1, StateId = 99 };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.StateId);
    }
}