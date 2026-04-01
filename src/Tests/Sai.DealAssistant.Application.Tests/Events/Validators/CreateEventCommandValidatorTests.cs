using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using Sai.DealAssistant.Application.Entities.Events.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Events.Validators
{
    public class CreateEventCommandValidatorTests
    {
        // Shared helper: builds a validator with all dependencies mocked to valid defaults.
        private static (
            Mock<IEnumCache<EventType>> type,
            Mock<IEnumCache<EventState>> state,
            Mock<IReadRepository<Deal>> deal,
            Mock<IReadRepository<ContactPerson>> cp,
            CreateEventCommand.Validator validator)
        BuildValidator(
            Deal? dealResult = null,
            bool cpExistsInFirm = false)
        {
            var typeCacheMock = new Mock<IEnumCache<EventType>>();
            var stateCacheMock = new Mock<IEnumCache<EventState>>();
            var dealRepoMock = new Mock<IReadRepository<Deal>>();
            var cpRepoMock = new Mock<IReadRepository<ContactPerson>>();

            typeCacheMock.Setup(c => c.GetAllAsync())
                .ReturnsAsync(new List<EventType> { new EventType { Id = 1 } });
            stateCacheMock.Setup(c => c.GetAllAsync())
                .ReturnsAsync(new List<EventState> { new EventState { Id = 2 } });

            dealRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(dealResult);

            // The validator calls ExistsAsync once: c => c.Id == contactPersonId && c.FirmId == deal.FirmId
            cpRepoMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<ContactPerson, bool>>>()))
                .ReturnsAsync(cpExistsInFirm);

            var validator = new CreateEventCommand.Validator(
                stateCacheMock.Object,
                typeCacheMock.Object,
                dealRepoMock.Object,
                cpRepoMock.Object);

            return (typeCacheMock, stateCacheMock, dealRepoMock, cpRepoMock, validator);
        }

        [Fact]
        public async Task Validator_WithValidData_NoContactPerson_PassesValidation()
        {
            var (_, _, _, _, validator) = BuildValidator(
                dealResult: new Deal { Id = 10, FirmId = 1 });

            var cmd = new CreateEventCommand
            {
                DealId = 10,
                TypeId = 1,
                StateId = 2,
                Topic = "Discovery call",
                Date = DateTimeOffset.UtcNow,
                ContactPersonId = null
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldNotHaveValidationErrorFor(c => c.DealId);
            result.ShouldNotHaveValidationErrorFor(c => c.TypeId);
            result.ShouldNotHaveValidationErrorFor(c => c.StateId);
            result.ShouldNotHaveValidationErrorFor(c => c.Date);
            result.ShouldNotHaveValidationErrorFor(c => c.ContactPersonId);
        }

        [Fact]
        public async Task Validator_InvalidTypeId_FailsValidation()
        {
            var (type, _, _, _, validator) = BuildValidator(
                dealResult: new Deal { Id = 10, FirmId = 1 });

            // Override: empty type cache → any TypeId is invalid
            type.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventType>());

            var cmd = new CreateEventCommand
            {
                DealId = 10,
                TypeId = 99,
                StateId = 2,
                Topic = "Discovery call",
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.TypeId);
        }

        [Fact]
        public async Task Validator_InvalidStateId_FailsValidation()
        {
            var (_, state, _, _, validator) = BuildValidator(
                dealResult: new Deal { Id = 10, FirmId = 1 });

            // Override: empty state cache → any StateId is invalid
            state.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventState>());

            var cmd = new CreateEventCommand
            {
                DealId = 10,
                TypeId = 1,
                StateId = 99,
                Topic = "Discovery call",
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.StateId);
        }

        [Fact]
        public async Task Validator_InvalidDealId_FailsValidation()
        {
            // dealResult = null → deal not found
            // The DealId rule uses MustAsync that checks existence.
            var (_, _, _, _, validator) = BuildValidator(dealResult: null);

            var cmd = new CreateEventCommand
            {
                DealId = 999,
                TypeId = 1,
                StateId = 2,
                Topic = "Discovery call",
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            // The DealId validation error comes from the DealId rule's MustAsync
            result.ShouldHaveValidationErrorFor(c => c.DealId);
        }

        [Fact]
        public async Task Validator_MissingDate_FailsValidation()
        {
            var (_, _, _, _, validator) = BuildValidator(
                dealResult: new Deal { Id = 10, FirmId = 1 });

            var cmd = new CreateEventCommand
            {
                DealId = 10,
                TypeId = 1,
                StateId = 2,
                Topic = "Discovery call",
                Date = default // missing
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Date);
        }

        [Fact]
        public async Task Validator_MissingTopic_FailsValidation()
        {
            var (_, _, _, _, validator) = BuildValidator(
                dealResult: new Deal { Id = 10, FirmId = 1 });

            var cmd = new CreateEventCommand
            {
                DealId = 10,
                TypeId = 1,
                StateId = 2,
                Topic = string.Empty,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Topic);
        }

        [Fact]
        public async Task Validator_WithContactPersonId_SameFirmAsDeal_PassesValidation()
        {
            // Deal has FirmId = 3; ContactPerson exists in same firm → ExistsAsync returns true
            var (_, _, _, _, validator) = BuildValidator(
                dealResult: new Deal { Id = 10, FirmId = 3 },
                cpExistsInFirm: true);

            var cmd = new CreateEventCommand
            {
                DealId = 10,
                TypeId = 1,
                StateId = 2,
                Topic = "Discovery call",
                ContactPersonId = 5,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldNotHaveValidationErrorFor(c => c.ContactPersonId);
        }

        [Fact]
        public async Task Validator_WithContactPersonId_ContactPersonNotFound_FailsValidation()
        {
            // cpExistsInFirm = false → ExistsAsync returns false
            var (_, _, _, _, validator) = BuildValidator(
                dealResult: new Deal { Id = 10, FirmId = 3 },
                cpExistsInFirm: false);

            var cmd = new CreateEventCommand
            {
                DealId = 10,
                TypeId = 1,
                StateId = 2,
                Topic = "Discovery call",
                ContactPersonId = 99,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.ContactPersonId);
        }

        [Fact]
        public async Task Validator_WithContactPersonId_DealHasNoFirm_FailsValidation()
        {
            // Deal has FirmId = null → validator short-circuits to false before calling ExistsAsync
            var (_, _, _, _, validator) = BuildValidator(
                dealResult: new Deal { Id = 10, FirmId = null },
                cpExistsInFirm: false);

            var cmd = new CreateEventCommand
            {
                DealId = 10,
                TypeId = 1,
                StateId = 2,
                Topic = "Discovery call",
                ContactPersonId = 5,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.ContactPersonId);
        }

        [Fact]
        public async Task Validator_WithContactPersonId_DifferentFirm_FailsValidation()
        {
            // ContactPerson exists but in a different firm → ExistsAsync(c.Id == 7 && c.FirmId == 3) returns false
            var (_, _, _, _, validator) = BuildValidator(
                dealResult: new Deal { Id = 10, FirmId = 3 },
                cpExistsInFirm: false);

            var cmd = new CreateEventCommand
            {
                DealId = 10,
                TypeId = 1,
                StateId = 2,
                Topic = "Discovery call",
                ContactPersonId = 7,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.ContactPersonId);
        }
    }
}