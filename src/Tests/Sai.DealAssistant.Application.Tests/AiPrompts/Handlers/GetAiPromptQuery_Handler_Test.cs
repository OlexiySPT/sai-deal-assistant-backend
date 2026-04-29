using AutoMapper;
using Sai.DealAssistant.Application.Entities.AiPrompts.Queries;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System.Linq;
using System.Threading;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.AiPrompts.Handlers
{
    public class GetAiPromptQuery_Handler_Test : UnitTestBase
    {
        private readonly ReadRepository<AppDbContext, AiPrompt> _repo;

        public GetAiPromptQuery_Handler_Test()
            : base(seedTestData: false)
        {
            _repo = new ReadRepository<AppDbContext, AiPrompt>(DbContext);

            using var db = CreateNewDbContext();
            db.Set<AiPrompt>().Add(new AiPrompt { Key = "k1", Version = "1.0", Text = "t1" });
            db.SaveChanges();
        }

        [Fact]
        public async void Handler_ReturnsPromptById()
        {
            var handler = new GetAiPromptQuery.Handler(_repo, Mapper);
            var existing = DbContext.Set<AiPrompt>().First();

            var result = await handler.Handle(new GetAiPromptQuery(existing.Id), CancellationToken.None);

            Assert.Equal(existing.Id, result.Id);
            Assert.Equal(existing.Key, result.Key);
            Assert.Equal(existing.Version, result.Version);
            Assert.Equal(existing.Text, result.Text);
        }
    }
}
