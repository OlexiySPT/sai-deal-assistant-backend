using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Sai.DealAssistant.Application.Entities.DealContactReps.Commands;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.DealContactReps.Validators
{
    public class UpdateDealContactRepCommandValidatorTests
    {
        [Fact]
        public async Task Validator_WithValidData_PassesValidation()
        {
            var validator = new UpdateDealContactRepCommand.Validator();
            var cmd = new UpdateDealContactRepCommand { Id = 1, Name = "Rep", Email = "rep@example.com" };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldNotHaveValidationErrorFor(c => c.Id);
            result.ShouldNotHaveValidationErrorFor(c => c.Name);
            result.ShouldNotHaveValidationErrorFor(c => c.Email);
        }

        [Fact]
        public async Task Validator_InvalidId_FailsValidation()
        {
            var validator = new UpdateDealContactRepCommand.Validator();
            var cmd = new UpdateDealContactRepCommand { Id = 0, Name = "Rep", Email = "rep@example.com" };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Id);
        }

        [Fact]
        public async Task Validator_InvalidEmail_FailsValidation()
        {
            var validator = new UpdateDealContactRepCommand.Validator();
            var cmd = new UpdateDealContactRepCommand { Id = 1, Name = "Rep", Email = "bad-email" };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Email);
        }
    }
}