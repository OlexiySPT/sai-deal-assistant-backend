using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using Sai.DealAssistant.Application.Entities.EventNotes.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.EventNotes.Validators
{
    public class CreateUpdateEventNoteCommandValidatorTests
    {
        [Fact]
        public async Task CreateValidator_WithValidEvent_PassesValidation()
        {
            var eventRepoMock = new Mock<IReadRepository<Event>>();
            eventRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Event, bool>>>()))
                .ReturnsAsync(new Event { Id = 1 });

            var validator = new CreateEventNoteCommand.Validator(eventRepoMock.Object);
            var cmd = new CreateEventNoteCommand { EventId = 1, Text = "Note text", Order = 1 };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldNotHaveValidationErrorFor(c => c.EventId);
            result.ShouldNotHaveValidationErrorFor(c => c.Text);
        }

        [Fact]
        public async Task CreateValidator_InvalidEventId_FailsValidation()
        {
            var eventRepoMock = new Mock<IReadRepository<Event>>();
            eventRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Event, bool>>>()))
                .ReturnsAsync((Event?)null);

            var validator = new CreateEventNoteCommand.Validator(eventRepoMock.Object);
            var cmd = new CreateEventNoteCommand { EventId = 99, Text = "Note text", Order = 1 };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.EventId);
        }

        [Fact]
        public async Task CreateValidator_EmptyText_FailsValidation()
        {
            var eventRepoMock = new Mock<IReadRepository<Event>>();
            eventRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Event, bool>>>()))
                .ReturnsAsync(new Event { Id = 1 });

            var validator = new CreateEventNoteCommand.Validator(eventRepoMock.Object);
            var cmd = new CreateEventNoteCommand { EventId = 1, Text = string.Empty, Order = 1 };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Text);
        }

        [Fact]
        public async Task UpdateValidator_WithValidData_PassesValidation()
        {
            var validator = new UpdateEventNoteCommand.Validator();
            var cmd = new UpdateEventNoteCommand { Id = 1, Text = "Updated", Order = 2, EventId = 1 };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldNotHaveValidationErrorFor(c => c.Id);
            result.ShouldNotHaveValidationErrorFor(c => c.Text);
        }

        [Fact]
        public async Task UpdateValidator_InvalidId_FailsValidation()
        {
            var validator = new UpdateEventNoteCommand.Validator();
            var cmd = new UpdateEventNoteCommand { Id = 0, Text = "Updated", Order = 2, EventId = 1 };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Id);
        }

        [Fact]
        public async Task UpdateValidator_EmptyText_FailsValidation()
        {
            var validator = new UpdateEventNoteCommand.Validator();
            var cmd = new UpdateEventNoteCommand { Id = 1, Text = string.Empty, Order = 2, EventId = 1 };

            var result = await validator.TestValidateAsync(cmd);

            result.ShouldHaveValidationErrorFor(c => c.Text);
        }
    }
}