using Sai.DealAssistant.Application.Entities.General.Commands;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.General.Commands.Validators
{
    public class UpdateNumericFieldCommandValidatorTests
    {
        [Fact]
        public void Validator_NotNull_Fails()
        {
            var command = new UpdateNumericFieldCommand
            {
                Entity = "Deal",
                Field = "ProposalAmount",
                Id = 1,
                Value = null,
                NotNull = true
            };

            var validator = new UpdateNumericFieldCommand.Validator();
            var validationResult = validator.Validate(command);
            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.ErrorMessage.Contains("Value must not be null."));
        }

        [Fact]
        public void Validator_Valid_Passes()
        {
            var command = new UpdateNumericFieldCommand
            {
                Entity = "Deal",
                Field = "ProposalAmount",
                Id = 1,
                Value = 123.45m,
                NotNull = true
            };

            var validator = new UpdateNumericFieldCommand.Validator();
            var validationResult = validator.Validate(command);
            Assert.True(validationResult.IsValid);
        }
    }
}