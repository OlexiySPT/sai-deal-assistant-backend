using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using Sai.DealAssistant.Application.Entities.AiPrompts.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.AiPrompts.Validators;

public class CreateAiPromptCommandValidatorTests
{
    private static Mock<IReadRepository<AiPrompt>> CreatePromptRepoMock(bool exists)
    {
        var mock = new Mock<IReadRepository<AiPrompt>>();
        mock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AiPrompt, bool>>>() ))
            .ReturnsAsync(exists);
        return mock;
    }

    [Fact]
    public async Task Validator_WithValidData_PassesValidation()
    {
        var repoMock = CreatePromptRepoMock(false);
        var validator = new CreateAiPromptCommand.Validator(repoMock.Object);
        var command = new CreateAiPromptCommand { Key = "key1", Version = "1.0", Text = "hello" };

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Key);
        result.ShouldNotHaveValidationErrorFor(c => c.Version);
        result.ShouldNotHaveValidationErrorFor(c => c.Text);
    }

    [Fact]
    public async Task Validator_DuplicateKeyVersion_FailsValidation()
    {
        var repoMock = CreatePromptRepoMock(true);
        var validator = new CreateAiPromptCommand.Validator(repoMock.Object);
        var command = new CreateAiPromptCommand { Key = "key1", Version = "1.0", Text = "hello" };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Key);
    }

    [Fact]
    public async Task Validator_InvalidVersion_FailsValidation()
    {
        var repoMock = CreatePromptRepoMock(false);
        var validator = new CreateAiPromptCommand.Validator(repoMock.Object);
        var command = new CreateAiPromptCommand { Key = "key1", Version = "abc", Text = "hello" };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Version);
    }

    [Fact]
    public async Task Validator_EmptyText_FailsValidation()
    {
        var repoMock = CreatePromptRepoMock(false);
        var validator = new CreateAiPromptCommand.Validator(repoMock.Object);
        var command = new CreateAiPromptCommand { Key = "key1", Version = "1.0", Text = "" };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Text);
    }
}
