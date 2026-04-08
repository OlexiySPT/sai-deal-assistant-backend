using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using Xunit;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Application.Entities.ContactPersons.Dto;
using Sai.DealAssistant.Application.Entities.ContactPersons.Queries;

namespace Sai.DealAssistant.Application.Tests.DealContactReps.Handlers
{
    public class GetContactPersonQuery_Handler_Test : UnitTestBase
    {
        private readonly ReadRepository<AppDbContext, ContactPerson> _repo;
        private readonly IMapper _mapper;

        public GetContactPersonQuery_Handler_Test()
            : base(seedTestData: false)
        {
            _repo = new ReadRepository<AppDbContext, ContactPerson>(DbContext);

            // Configure mapper for ContactPerson -> ContactPersonDto
            var cfg = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ContactPersonDto.MappingProfile>();
            }, LoggerFactory);
            _mapper = cfg.CreateMapper();

            // Seed a single contact person under a firm
            using (var db = CreateNewDbContext())
            {
                var firm = new Firm { Name = "Test Firm", Country = "USA" };
                db.Firms.Add(firm);
                db.SaveChanges();

                var contactPerson = new ContactPerson
                {
                    Name = "John Doe",
                    Email = "john@example.com",
                    FirmId = firm.Id,
                };
                db.ContactPersons.Add(contactPerson);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task Handler_ReturnsDto_ForExistingId()
        {
            var existing = await DbContext.ContactPersons.FirstAsync();
            var handler = new GetContactPersonQuery.Handler(_repo, _mapper);

            var result = await handler.Handle(new GetContactPersonQuery(existing.Id), CancellationToken.None);

            Assert.Equal(existing.Id, result.Id);
            Assert.Equal(existing.Name, result.Name);
            Assert.Equal(existing.Email, result.Email);
        }

        [Fact]
        public async Task Handler_ThrowsNotFound_ForMissingId()
        {
            var handler = new GetContactPersonQuery.Handler(_repo, _mapper);

            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() =>
                handler.Handle(new GetContactPersonQuery(9999), CancellationToken.None));
        }
    }
}