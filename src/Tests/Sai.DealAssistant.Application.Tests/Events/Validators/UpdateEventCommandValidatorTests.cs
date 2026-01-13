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
    public class UpdateEventCommandValidatorTests
    {
        [Fact]
        public async Task Validator_WithValidData_PassesValidation()
        {
            var typeCacheMock = new Mock<IEnumCache<EventType>>();
            var stateCacheMock = new Mock<IEnumCache<EventState>>();
            var contactPersonRepoMock = new Mock<ICrudRepository<ContactPerson>>();
            var eventRepoMock = new Mock<ICrudRepository<Event>>();

            typeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventType> { new EventType { Id = 1 } });
            stateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventState> { new EventState { Id = 2 } });
            contactPersonRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<ContactPerson, bool>>>()))
                .ReturnsAsync((ContactPerson?)null);

            var validator = new UpdateEventCommand.Validator(
                stateCacheMock.Object,
                typeCacheMock.Object,
                contactPersonRepoMock.Object,
                eventRepoMock.Object
            );

            var cmd = new UpdateEventCommand
            {
                Id = 5,
                TypeId = 1,
                StateId = 2,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldNotHaveValidationErrorFor(c => c.Id);
            result.ShouldNotHaveValidationErrorFor(c => c.TypeId);
            result.ShouldNotHaveValidationErrorFor(c => c.StateId);
            result.ShouldNotHaveValidationErrorFor(c => c.Date);
        }

        [Fact]
        public async Task Validator_InvalidId_FailsValidation()
        {
            var typeCacheMock = new Mock<IEnumCache<EventType>>();
            var stateCacheMock = new Mock<IEnumCache<EventState>>();
            var contactPersonRepoMock = new Mock<ICrudRepository<ContactPerson>>();
            var eventRepoMock = new Mock<ICrudRepository<Event>>();

            typeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventType> { new EventType { Id = 1 } });
            stateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventState> { new EventState { Id = 2 } });
            contactPersonRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<ContactPerson, bool>>>()))
                .ReturnsAsync((ContactPerson?)null);

            var validator = new UpdateEventCommand.Validator(
                stateCacheMock.Object,
                typeCacheMock.Object,
                contactPersonRepoMock.Object,
                eventRepoMock.Object
            );

            var cmd = new UpdateEventCommand
            {
                Id = 0,
                TypeId = 1,
                StateId = 2,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Id);
        }

        [Fact]
        public async Task Validator_InvalidTypeId_FailsValidation()
        {
            var typeCacheMock = new Mock<IEnumCache<EventType>>();
            var stateCacheMock = new Mock<IEnumCache<EventState>>();
            var contactPersonRepoMock = new Mock<ICrudRepository<ContactPerson>>();
            var eventRepoMock = new Mock<ICrudRepository<Event>>();

            typeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventType>()); // no types
            stateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventState> { new EventState { Id = 2 } });
            contactPersonRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<ContactPerson, bool>>>()))
                .ReturnsAsync((ContactPerson?)null);

            var validator = new UpdateEventCommand.Validator(
                stateCacheMock.Object,
                typeCacheMock.Object,
                contactPersonRepoMock.Object,
                eventRepoMock.Object
            );

            var cmd = new UpdateEventCommand
            {
                Id = 5,
                TypeId = 99,
                StateId = 2,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.TypeId);
        }

        [Fact]
        public async Task Validator_InvalidStateId_FailsValidation()
        {
            var typeCacheMock = new Mock<IEnumCache<EventType>>();
            var stateCacheMock = new Mock<IEnumCache<EventState>>();
            var contactPersonRepoMock = new Mock<ICrudRepository<ContactPerson>>();
            var eventRepoMock = new Mock<ICrudRepository<Event>>();

            typeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventType> { new EventType { Id = 1 } });
            stateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventState>()); // no states
            contactPersonRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<ContactPerson, bool>>>()))
                .ReturnsAsync((ContactPerson?)null);

            var validator = new UpdateEventCommand.Validator(
                stateCacheMock.Object,
                typeCacheMock.Object,
                contactPersonRepoMock.Object,
                eventRepoMock.Object
            );

            var cmd = new UpdateEventCommand
            {
                Id = 5,
                TypeId = 1,
                StateId = 99,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.StateId);
        }

        [Fact]
        public async Task Validator_MissingDate_FailsValidation()
        {
            var typeCacheMock = new Mock<IEnumCache<EventType>>();
            var stateCacheMock = new Mock<IEnumCache<EventState>>();
            var contactPersonRepoMock = new Mock<ICrudRepository<ContactPerson>>();
            var eventRepoMock = new Mock<ICrudRepository<Event>>();

            typeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventType> { new EventType { Id = 1 } });
            stateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventState> { new EventState { Id = 2 } });
            contactPersonRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<ContactPerson, bool>>>()))
                .ReturnsAsync((ContactPerson?)null);

            var validator = new UpdateEventCommand.Validator(
                stateCacheMock.Object,
                typeCacheMock.Object,
                contactPersonRepoMock.Object,
                eventRepoMock.Object
            );

            var cmd = new UpdateEventCommand
            {
                Id = 5,
                TypeId = 1,
                StateId = 2,
                Date = default
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Date);
        }

        [Fact]
        public async Task Validator_WithContactPersonId_AndExists_PassesValidation()
        {
            var typeCacheMock = new Mock<IEnumCache<EventType>>();
            var stateCacheMock = new Mock<IEnumCache<EventState>>();
            var contactPersonRepoMock = new Mock<ICrudRepository<ContactPerson>>();
            var eventRepoMock = new Mock<ICrudRepository<Event>>();

            typeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventType> { new EventType { Id = 1 } });
            stateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventState> { new EventState { Id = 2 } });
            // contact person exists
            contactPersonRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<ContactPerson, bool>>>()))
                .ReturnsAsync(true);

            var validator = new UpdateEventCommand.Validator(
                stateCacheMock.Object,
                typeCacheMock.Object,
                contactPersonRepoMock.Object,
                eventRepoMock.Object
            );

            var cmd = new UpdateEventCommand
            {
                Id = 5,
                TypeId = 1,
                StateId = 2,
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
        public async Task Validator_WithContactPersonId_NotFound_FailsValidation()
        {
            var typeCacheMock = new Mock<IEnumCache<EventType>>();
            var stateCacheMock = new Mock<IEnumCache<EventState>>();
            var contactPersonRepoMock = new Mock<ICrudRepository<ContactPerson>>();
            var eventRepoMock = new Mock<ICrudRepository<Event>>();

            typeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventType> { new EventType { Id = 1 } });
            stateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventState> { new EventState { Id = 2 } });
            // contact person not found
            contactPersonRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<ContactPerson, bool>>>()))
                .ReturnsAsync((ContactPerson?)null);

            var validator = new UpdateEventCommand.Validator(
                stateCacheMock.Object,
                typeCacheMock.Object,
                contactPersonRepoMock.Object,
                eventRepoMock.Object
            );

            var cmd = new UpdateEventCommand
            {
                Id = 5,
                TypeId = 1,
                StateId = 2,
                ContactPersonId = 99,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.ContactPersonId);
        }
    }
}