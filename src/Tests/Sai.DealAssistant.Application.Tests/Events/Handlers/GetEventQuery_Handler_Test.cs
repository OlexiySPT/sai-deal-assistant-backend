using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Entities.Events.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using Xunit;
using Sai.DealAssistant.Application.Common.Exceptions;
using System;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Application.Entities.Events.Queries;

namespace Sai.DealAssistant.Application.Tests.Events.Handlers
{
	public class GetEventQuery_Handler_Test : UnitTestBase
	{
		private readonly ReadRepository<AppDbContext, Event> _repo;
		private readonly IMapper _mapper;

		public GetEventQuery_Handler_Test()
			: base(seedTestData: false)
		{
			_repo = new ReadRepository<AppDbContext, Event>(DbContext);

			var cfg = new MapperConfiguration(cfg =>
			{
				cfg.AddProfile<EventDto.MappingProfile>();
			}, LoggerFactory);
			_mapper = cfg.CreateMapper();

			// seed event and required type/state
			using (var db = CreateNewDbContext())
			{
				var dt = db.DealTypes.Add(new DealType { Type = "Standard" });
				var ds = db.DealStates.Add(new DealState { State = "Open" });
				db.SaveChanges();
                var deal = new Deal { Name = "Test Deal", TypeId = dt.Entity.Id, StateId = ds.Entity.Id };
				db.Deals.Add(deal);
				db.SaveChanges();

				var et = new EventType { Name = "Summary" };
				var es = new EventState { State = "Done" };
				db.AddRange(et, es);
				db.SaveChanges();

				db.Events.Add(new Event { Date = DateTimeOffset.UtcNow, Agenda = "Meet", DealId = deal.Id, TypeId = et.Id, StateId = es.Id });
				db.SaveChanges();
			}
		}

		[Fact]
		public async Task Handler_ReturnsDto_ForExistingId()
		{
			var existing = await DbContext.Events.FirstAsync();
			var handler = new GetEventQuery.Handler(_repo, _mapper);

			var result = await handler.Handle(new GetEventQuery(existing.Id), CancellationToken.None);

			Assert.Equal(existing.Id, result.Id);
			Assert.Equal(existing.Agenda, result.Agenda);
			Assert.Equal(existing.Date, result.Date);
		}

		[Fact]
		public async Task Handler_ThrowsNotFound_ForMissingId()
		{
			var handler = new GetEventQuery.Handler(_repo, _mapper);

			await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(new GetEventQuery(9999), CancellationToken.None));
		}
	}
}