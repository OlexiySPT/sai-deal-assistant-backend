using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Entities.EventNotes.Queries;
using Sai.DealAssistant.Application.Entities.EventNotes.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

namespace Tests.Sai.DealAssistant.Application.Tests.EventNotes.Handlers
{
    public class GetEventNoteQuery_Handler_Test : UnitTestBase
    {
        private readonly ReadRepository<AppDbContext, EventNote> _repo;
        private readonly IMapper _mapper;

        public GetEventNoteQuery_Handler_Test()
            : base(seedTestData: false)
        {
            _repo = new ReadRepository<AppDbContext, EventNote>(DbContext);

            // Configure AutoMapper with the EventNoteDto.MappingProfile
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EventNoteDto.MappingProfile>();
            }, LoggerFactory);
            _mapper = config.CreateMapper();

            // Seed test data
            using (var db = CreateNewDbContext())
            {
                var state = db.DealStates.Add(new DealState { State = "Open" });
                var type = db.DealTypes.Add(new DealType { Type = "Standard" });
                var eventtype = db.EventTypes.Add(new EventType { Name = "Meeting" });
                var eventstate = db.EventStates.Add(new EventState { State = "Scheduled" });
                db.SaveChanges();

                var deal = new Deal { Name = "Deal A", StateId = state.Entity.Id, TypeId = type.Entity.Id };
                db.Add(deal);
                db.SaveChanges();

                var ev = new Event { Agenda = "Event A", DealId = deal.Id, TypeId = eventtype.Entity.Id, StateId = eventstate.Entity.Id };
                db.Add(ev);
                db.SaveChanges();

                var note = new EventNote { Text = "SingleNote", Order = 1, EventId = ev.Id };
                db.EventNotes.Add(note);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task Handler_ReturnsCorrectEventNoteDto_WhenExists()
        {
            // Arrange
            var handler = new GetEventNoteQuery.Handler(_repo, _mapper);

            var note = DbContext.EventNotes.First();
            var query = new GetEventNoteQuery(note.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(note.Id, result.Id);
            Assert.Equal(note.Order, result.Order);
            Assert.Equal(note.Text, result.Text);
            Assert.Equal(note.EventId, result.EventId);
        }
    }
}