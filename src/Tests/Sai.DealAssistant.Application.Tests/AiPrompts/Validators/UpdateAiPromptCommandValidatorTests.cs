using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using Sai.DealAssistant.Application.Entities.AiPrompts.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.AiPrompts.Validators;

public class UpdateAiPromptCommandValidatorTests
{
    private static Mock<IReadRepository<AiPrompt>> CreatePromptRepoMock()
    {
        var mock = new Mock<IReadRepository<AiPrompt>>();
        return mock;
    }

    [Fact]
    public async Task Validator_WithValidData_PassesValidation()
    {
        var repoMock = CreatePromptRepoMock();
        var validator = new UpdateAiPromptCommand.Validator(repoMock.Object);
        var command = new UpdateAiPromptCommand { Id = 1, Key = "k", Version = "1.0", Text = "t" };

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Id);
        result.ShouldNotHaveValidationErrorFor(c => c.Key);
        result.ShouldNotHaveValidationErrorFor(c => c.Version);
        result.ShouldNotHaveValidationErrorFor(c => c.Text);
    }

    [Fact]
    public async Task Validator_InvalidVersion_FailsValidation()
    {
        var repoMock = CreatePromptRepoMock();
        var validator = new UpdateAiPromptCommand.Validator(repoMock.Object);
        var command = new UpdateAiPromptCommand { Id = 1, Key = "k", Version = "x", Text = "t" };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Version);
    }

    [Fact]
    public async Task Validator_EmptyText_FailsValidation()
    {
        var repoMock = CreatePromptRepoMock();
        var validator = new UpdateAiPromptCommand.Validator(repoMock.Object);
        var command = new UpdateAiPromptCommand { Id = 1, Key = "k", Version = "1.0", Text = "" };

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Text);
    }
}
