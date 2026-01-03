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
    public class CreateDealContactRepCommandValidatorTests
    {
        [Fact]
        public async Task Validator_WithValidDeal_PassesValidation()
        {
            var dealRepoMock = new Mock<IReadRepository<Deal>>();
            dealRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(new Deal { Id = 1 });

            var validator = new CreateContactPersonCommand.Validator(dealRepoMock.Object);
            var cmd = new CreateContactPersonCommand { DealId = 1, Name = "Rep", Email = "rep@example.com" };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldNotHaveValidationErrorFor(c => c.DealId);
            result.ShouldNotHaveValidationErrorFor(c => c.Name);
            result.ShouldNotHaveValidationErrorFor(c => c.Email);
        }

        [Fact]
        public async Task Validator_InvalidDealId_FailsValidation()
        {
            var dealRepoMock = new Mock<IReadRepository<Deal>>();
            dealRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync((Deal?)null);

            var validator = new CreateContactPersonCommand.Validator(dealRepoMock.Object);
            var cmd = new CreateContactPersonCommand { DealId = 99, Name = "Rep", Email = "rep@example.com" };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.DealId);
        }

        [Fact]
        public async Task Validator_InvalidEmail_FailsValidation()
        {
            var dealRepoMock = new Mock<IReadRepository<Deal>>();
            dealRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(new Deal { Id = 1 });

            var validator = new CreateContactPersonCommand.Validator(dealRepoMock.Object);
            var cmd = new CreateContactPersonCommand { DealId = 1, Name = "Rep", Email = "not-an-email" };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Fact]
        public async Task Validator_EmailTooLong_FailsValidation()
        {
            var dealRepoMock = new Mock<IReadRepository<Deal>>();
            dealRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(new Deal { Id = 1 });

            var validator = new CreateContactPersonCommand.Validator(dealRepoMock.Object);
            var longLocal = new string('a', 151);
            var longEmail = $"{longLocal}@ex.com"; // > 150 chars in total
            var cmd = new CreateContactPersonCommand { DealId = 1, Name = "Rep", Email = longEmail };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Email);
        }
    }
}