using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UpdateEventCommandValidatorTests
    {
        private const int ExistingEventId = 5;
        private const int ExistingDealId = 10;
        private const int ExistingFirmId = 3;

        /// <summary>
        /// Builds a validator with all dependencies mocked to valid defaults.
        /// </summary>
        private static (
            Mock<IEnumCache<EventType>> type,
            Mock<IEnumCache<EventState>> state,
            Mock<IReadRepository<ContactPerson>> cp,
            Mock<IReadRepository<Event>> evt,
            Mock<IReadRepository<Deal>> deal,
            UpdateEventCommand.Validator validator)
        BuildValidator(bool cpExistsInFirm = false)
        {
            var typeCacheMock  = new Mock<IEnumCache<EventType>>();
            var stateCacheMock = new Mock<IEnumCache<EventState>>();
            var cpRepoMock     = new Mock<IReadRepository<ContactPerson>>();
            var evtRepoMock    = new Mock<IReadRepository<Event>>();
            var dealRepoMock   = new Mock<IReadRepository<Deal>>();

            typeCacheMock.Setup(c => c.GetAllAsync())
                .ReturnsAsync(new List<EventType> { new EventType { Id = 1 } });
            stateCacheMock.Setup(c => c.GetAllAsync())
                .ReturnsAsync(new List<EventState> { new EventState { Id = 2 } });

            // Simulate resolving DealId from the existing event via GetAll()
            evtRepoMock.Setup(r => r.GetAll())
                .Returns(new List<Event>
                {
                    new Event { Id = ExistingEventId, DealId = ExistingDealId }
                }.AsQueryable());

            // The deal has FirmId assigned
            dealRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(new Deal { Id = ExistingDealId, FirmId = ExistingFirmId });

            // The validator calls ExistsAsync once: c => c.Id == contactPersonId && c.FirmId == deal.FirmId
            cpRepoMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<ContactPerson, bool>>>()))
                .ReturnsAsync(cpExistsInFirm);

            var validator = new UpdateEventCommand.Validator(
                stateCacheMock.Object,
                typeCacheMock.Object,
                cpRepoMock.Object,
                evtRepoMock.Object,
                dealRepoMock.Object);

            return (typeCacheMock, stateCacheMock, cpRepoMock, evtRepoMock, dealRepoMock, validator);
        }

        [Fact]
        public async Task Validator_WithValidData_NoContactPerson_PassesValidation()
        {
            var (_, _, _, _, _, validator) = BuildValidator();

            var cmd = new UpdateEventCommand
            {
                Id = ExistingEventId,
                TypeId = 1,
                StateId = 2,
                Topic = "Updated topic",
                Date = DateTimeOffset.UtcNow,
                ContactPersonId = null
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldNotHaveValidationErrorFor(c => c.Id);
            result.ShouldNotHaveValidationErrorFor(c => c.TypeId);
            result.ShouldNotHaveValidationErrorFor(c => c.StateId);
            result.ShouldNotHaveValidationErrorFor(c => c.Date);
            result.ShouldNotHaveValidationErrorFor(c => c.ContactPersonId);
        }

        [Fact]
        public async Task Validator_InvalidId_FailsValidation()
        {
            var (_, _, _, _, _, validator) = BuildValidator();

            var cmd = new UpdateEventCommand
            {
                Id = 0, // invalid
                TypeId = 1,
                StateId = 2,
                Topic = "Updated topic",
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Id);
        }

        [Fact]
        public async Task Validator_InvalidTypeId_FailsValidation()
        {
            var (type, _, _, _, _, validator) = BuildValidator();

            type.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventType>());

            var cmd = new UpdateEventCommand
            {
                Id = ExistingEventId,
                TypeId = 99,
                StateId = 2,
                Topic = "Updated topic",
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.TypeId);
        }

        [Fact]
        public async Task Validator_InvalidStateId_FailsValidation()
        {
            var (_, state, _, _, _, validator) = BuildValidator();

            state.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventState>());

            var cmd = new UpdateEventCommand
            {
                Id = ExistingEventId,
                TypeId = 1,
                StateId = 99,
                Topic = "Updated topic",
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.StateId);
        }

        [Fact]
        public async Task Validator_MissingDate_FailsValidation()
        {
            var (_, _, _, _, _, validator) = BuildValidator();

            var cmd = new UpdateEventCommand
            {
                Id = ExistingEventId,
                TypeId = 1,
                StateId = 2,
                Topic = "Updated topic",
                Date = default
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Date);
        }

        [Fact]
        public async Task Validator_MissingTopic_FailsValidation()
        {
            var (_, _, _, _, _, validator) = BuildValidator();

            var cmd = new UpdateEventCommand
            {
                Id = ExistingEventId,
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
            // ExistsAsync(c => c.Id == 5 && c.FirmId == 3) returns true
            var (_, _, _, _, _, validator) = BuildValidator(cpExistsInFirm: true);

            var cmd = new UpdateEventCommand
            {
                Id = ExistingEventId,
                TypeId = 1,
                StateId = 2,
                Topic = "Updated topic",
                ContactPersonId = 5,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldNotHaveValidationErrorFor(c => c.ContactPersonId);
            result.ShouldNotHaveValidationErrorFor(c => c.Id);
            result.ShouldNotHaveValidationErrorFor(c => c.TypeId);
            result.ShouldNotHaveValidationErrorFor(c => c.StateId);
            result.ShouldNotHaveValidationErrorFor(c => c.Date);
        }

        [Fact]
        public async Task Validator_WithContactPersonId_ContactPersonNotFound_FailsValidation()
        {
            // ExistsAsync returns false
            var (_, _, _, _, _, validator) = BuildValidator(cpExistsInFirm: false);

            var cmd = new UpdateEventCommand
            {
                Id = ExistingEventId,
                TypeId = 1,
                StateId = 2,
                Topic = "Updated topic",
                ContactPersonId = 99,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.ContactPersonId);
        }

        [Fact]
        public async Task Validator_WithContactPersonId_DealHasNoFirm_FailsValidation()
        {
            // Override deal to have no firm → validator short-circuits to false
            var (_, _, _, _, deal, validator) = BuildValidator(cpExistsInFirm: false);

            deal.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(new Deal { Id = ExistingDealId, FirmId = null });

            var cmd = new UpdateEventCommand
            {
                Id = ExistingEventId,
                TypeId = 1,
                StateId = 2,
                Topic = "Updated topic",
                ContactPersonId = 5,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.ContactPersonId);
        }

        [Fact]
        public async Task Validator_WithContactPersonId_DifferentFirm_FailsValidation()
        {
            // ExistsAsync(c.Id == 7 && c.FirmId == 3) returns false because CP is in a different firm
            var (_, _, _, _, _, validator) = BuildValidator(cpExistsInFirm: false);

            var cmd = new UpdateEventCommand
            {
                Id = ExistingEventId,
                TypeId = 1,
                StateId = 2,
                Topic = "Updated topic",
                ContactPersonId = 7,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.ContactPersonId);
        }
    }
}