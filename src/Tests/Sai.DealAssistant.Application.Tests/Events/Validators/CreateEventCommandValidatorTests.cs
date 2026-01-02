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
        [Fact]
        public async Task Validator_WithValidData_PassesValidation()
        {
            var typeCacheMock = new Mock<IEnumCache<EventType>>();
            var stateCacheMock = new Mock<IEnumCache<EventState>>();
            var dealRepoMock = new Mock<IReadRepository<Deal>>();

            typeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventType> { new EventType { Id = 1 } });
            stateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventState> { new EventState { Id = 2 } });
            dealRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(new Deal { Id = 10 });

            var validator = new CreateEventCommand.Validator(stateCacheMock.Object, typeCacheMock.Object, dealRepoMock.Object);

            var cmd = new CreateEventCommand
            {
                DealId = 10,
                TypeId = 1,
                StateId = 2,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldNotHaveValidationErrorFor(c => c.DealId);
            result.ShouldNotHaveValidationErrorFor(c => c.TypeId);
            result.ShouldNotHaveValidationErrorFor(c => c.StateId);
            result.ShouldNotHaveValidationErrorFor(c => c.Date);
        }

        [Fact]
        public async Task Validator_InvalidTypeId_FailsValidation()
        {
            var typeCacheMock = new Mock<IEnumCache<EventType>>();
            var stateCacheMock = new Mock<IEnumCache<EventState>>();
            var dealRepoMock = new Mock<IReadRepository<Deal>>();

            typeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventType>()); // empty -> invalid
            stateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventState> { new EventState { Id = 2 } });
            dealRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(new Deal { Id = 10 });

            var validator = new CreateEventCommand.Validator(stateCacheMock.Object, typeCacheMock.Object, dealRepoMock.Object);

            var cmd = new CreateEventCommand
            {
                DealId = 10,
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
            var dealRepoMock = new Mock<IReadRepository<Deal>>();

            typeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventType> { new EventType { Id = 1 } });
            stateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventState>()); // empty -> invalid
            dealRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(new Deal { Id = 10 });

            var validator = new CreateEventCommand.Validator(stateCacheMock.Object, typeCacheMock.Object, dealRepoMock.Object);

            var cmd = new CreateEventCommand
            {
                DealId = 10,
                TypeId = 1,
                StateId = 99,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.StateId);
        }

        [Fact]
        public async Task Validator_InvalidDealId_FailsValidation()
        {
            var typeCacheMock = new Mock<IEnumCache<EventType>>();
            var stateCacheMock = new Mock<IEnumCache<EventState>>();
            var dealRepoMock = new Mock<IReadRepository<Deal>>();

            typeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventType> { new EventType { Id = 1 } });
            stateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventState> { new EventState { Id = 2 } });
            dealRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync((Deal?)null); // deal not found

            var validator = new CreateEventCommand.Validator(stateCacheMock.Object, typeCacheMock.Object, dealRepoMock.Object);

            var cmd = new CreateEventCommand
            {
                DealId = 999,
                TypeId = 1,
                StateId = 2,
                Date = DateTimeOffset.UtcNow
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.DealId);
        }

        [Fact]
        public async Task Validator_MissingDate_FailsValidation()
        {
            var typeCacheMock = new Mock<IEnumCache<EventType>>();
            var stateCacheMock = new Mock<IEnumCache<EventState>>();
            var dealRepoMock = new Mock<IReadRepository<Deal>>();

            typeCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventType> { new EventType { Id = 1 } });
            stateCacheMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<EventState> { new EventState { Id = 2 } });
            dealRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Deal, bool>>>()))
                .ReturnsAsync(new Deal { Id = 10 });

            var validator = new CreateEventCommand.Validator(stateCacheMock.Object, typeCacheMock.Object, dealRepoMock.Object);

            var cmd = new CreateEventCommand
            {
                DealId = 10,
                TypeId = 1,
                StateId = 2,
                Date = default // missing date
            };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Date);
        }
    }
}