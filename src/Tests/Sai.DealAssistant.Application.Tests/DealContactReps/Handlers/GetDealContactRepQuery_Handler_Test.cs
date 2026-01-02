using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Entities.DealContactReps.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using Xunit;
using Sai.DealAssistant.Application.Common.Exceptions;

namespace Sai.DealAssistant.Application.Tests.DealContactReps.Handlers
{
    public class GetDealContactRepQuery_Handler_Test : UnitTestBase
    {
        private readonly ReadRepository<AppDbContext, DealContactRep> _repo;
        private readonly IMapper _mapper;

        public GetDealContactRepQuery_Handler_Test()
            : base(seedTestData: false)
        {
            _repo = new ReadRepository<AppDbContext, DealContactRep>(DbContext);

            // Configure mapper for DealContactRep -> DealContactRepDto
            var cfg = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DealContactRepDto.MappingProfile>();
            }, LoggerFactory);
            _mapper = cfg.CreateMapper();

            // Seed single rep
            using (var db = CreateNewDbContext())
            {
                var deal = new Deal { Name = "Test Deal" };
                db.Deals.Add(deal);
                db.SaveChanges();

                db.DealContactReps.Add(new DealContactRep { Name = "John Doe", Email = "john@example.com", DealId = deal.Id });
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task Handler_ReturnsDto_ForExistingId()
        {
            var existing = await DbContext.DealContactReps.FirstAsync();
            var handler = new Sai.DealAssistant.Application.Entities.DealContactReps.Queries.GetDealContactRepQuery.Handler(_repo, _mapper);

            var result = await handler.Handle(new Sai.DealAssistant.Application.Entities.DealContactReps.Queries.GetDealContactRepQuery(existing.Id), CancellationToken.None);

            Assert.Equal(existing.Id, result.Id);
            Assert.Equal(existing.Name, result.Name);
            Assert.Equal(existing.Email, result.Email);
        }

        [Fact]
        public async Task Handler_ThrowsNotFound_ForMissingId()
        {
            var handler = new Sai.DealAssistant.Application.Entities.DealContactReps.Queries.GetDealContactRepQuery.Handler(_repo, _mapper);

            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(new Sai.DealAssistant.Application.Entities.DealContactReps.Queries.GetDealContactRepQuery(9999), CancellationToken.None));
        }
    }
}