using Sai.DealAssistant.Application.Entities.General.Commands;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.General.Commands.Validators
{
    public class UpdateStringFieldCommandValidatorTests
    {
        [Fact]
        public void Validator_NotEmpty_Fails()
        {
            var command = new UpdateStringFieldCommand
            {
                Entity = "Deal",
                Field = "Name",
                Id = 1,
                Value = "   ",
                Validation = StringFieldValidationType.NotEmpty
            };

            var validator = new UpdateStringFieldCommand.Validator();
            var validationResult = validator.Validate(command);
            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.ErrorMessage.Contains("Value must not be empty."));
        }

        [Fact]
        public void Validator_NotNull_Fails()
        {
            var command = new UpdateStringFieldCommand
            {
                Entity = "Deal",
                Field = "Name",
                Id = 1,
                Value = null,
                Validation = StringFieldValidationType.NotNull
            };

            var validator = new UpdateStringFieldCommand.Validator();
            var validationResult = validator.Validate(command);
            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.ErrorMessage.Contains("Value must not be null."));
        }

        [Fact]
        public void Validator_Email_Fails()
        {
            var command = new UpdateStringFieldCommand
            {
                Entity = "Deal",
                Field = "Name",
                Id = 1,
                Value = "not-an-email",
                Validation = StringFieldValidationType.Email
            };

            var validator = new UpdateStringFieldCommand.Validator();
            var validationResult = validator.Validate(command);
            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.ErrorMessage.Contains("Value must be a valid email address."));
        }

        [Fact]
        public void Validator_Url_Fails()
        {
            var command = new UpdateStringFieldCommand
            {
                Entity = "Deal",
                Field = "Name",
                Id = 1,
                Value = "not-a-url",
                Validation = StringFieldValidationType.Url
            };

            var validator = new UpdateStringFieldCommand.Validator();
            var validationResult = validator.Validate(command);
            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.ErrorMessage.Contains("Value must be a valid URL."));
        }
    }
}