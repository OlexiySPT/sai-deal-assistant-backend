using Sai.DealAssistant.Application.Entities.General.Commands;
using System;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.General.Commands.Validators
{
    public class UpdateDateFieldCommandValidatorTests
    {
        [Fact]
        public void Validator_NotNull_Fails()
        {
            var command = new UpdateDateFieldCommand
            {
                Entity = "Deal",
                Field = "CreatedAt",
                Id = 1,
                Value = null,
                NotNull = true
            };

            var validator = new UpdateDateFieldCommand.Validator();
            var validationResult = validator.Validate(command);
            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.ErrorMessage.Contains("Value must not be null."));
        }

        [Fact]
        public void Validator_Valid_Passes()
        {
            var command = new UpdateDateFieldCommand
            {
                Entity = "Deal",
                Field = "CreatedAt",
                Id = 1,
                Value = DateTimeOffset.UtcNow,
                NotNull = true
            };

            var validator = new UpdateDateFieldCommand.Validator();
            var validationResult = validator.Validate(command);
            Assert.True(validationResult.IsValid);
        }
    }
}