using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using Sai.DealAssistant.Application.Entities.ContactPersons.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.DealContactReps.Validators
{
    public class CreateContactPersonCommandValidatorTests
    {
        [Fact]
        public async Task Validator_WithValidFirm_PassesValidation()
        {
            var firmRepoMock = new Mock<IReadRepository<Firm>>();
            firmRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Firm, bool>>>()))
                .ReturnsAsync(new Firm { Id = 1, Name = "Test Firm", Country = "USA" });

            var validator = new CreateContactPersonCommand.Validator(firmRepoMock.Object);
            var cmd = new CreateContactPersonCommand { FirmId = 1, Name = "Rep", Email = "rep@example.com" };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldNotHaveValidationErrorFor(c => c.FirmId);
            result.ShouldNotHaveValidationErrorFor(c => c.Name);
            result.ShouldNotHaveValidationErrorFor(c => c.Email);
        }

        [Fact]
        public async Task Validator_InvalidFirmId_FailsValidation()
        {
            var firmRepoMock = new Mock<IReadRepository<Firm>>();
            firmRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Firm, bool>>>()))
                .ReturnsAsync((Firm?)null);

            var validator = new CreateContactPersonCommand.Validator(firmRepoMock.Object);
            var cmd = new CreateContactPersonCommand { FirmId = 99, Name = "Rep", Email = "rep@example.com" };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.FirmId);
        }

        [Fact]
        public async Task Validator_InvalidEmail_FailsValidation()
        {
            var firmRepoMock = new Mock<IReadRepository<Firm>>();
            firmRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Firm, bool>>>()))
                .ReturnsAsync(new Firm { Id = 1, Name = "Test Firm", Country = "USA" });

            var validator = new CreateContactPersonCommand.Validator(firmRepoMock.Object);
            var cmd = new CreateContactPersonCommand { FirmId = 1, Name = "Rep", Email = "not-an-email" };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Fact]
        public async Task Validator_EmailTooLong_FailsValidation()
        {
            var firmRepoMock = new Mock<IReadRepository<Firm>>();
            firmRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Firm, bool>>>()))
                .ReturnsAsync(new Firm { Id = 1, Name = "Test Firm", Country = "USA" });

            var validator = new CreateContactPersonCommand.Validator(firmRepoMock.Object);
            var longLocal = new string('a', 151);
            var longEmail = $"{longLocal}@ex.com"; // > 150 chars in total
            var cmd = new CreateContactPersonCommand { FirmId = 1, Name = "Rep", Email = longEmail };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Email);
        }
    }
}