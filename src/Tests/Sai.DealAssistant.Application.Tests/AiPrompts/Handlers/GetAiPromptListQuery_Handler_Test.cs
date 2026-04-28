using Sai.DealAssistant.Application.Entities.AiPrompts.Queries;
using Sai.DealAssistant.Application.Entities.AiPrompts.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System.Linq;
using System.Threading;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.AiPrompts.Handlers
{
    public class GetAiPromptListQuery_Handler_Test : UnitTestBase
    {
        private readonly ReadRepository<AppDbContext, AiPrompt> _repo;

        public GetAiPromptListQuery_Handler_Test()
            : base(seedTestData: false)
        {
            _repo = new ReadRepository<AppDbContext, AiPrompt>(DbContext);

            using var db = CreateNewDbContext();
            var items = Enumerable.Range(1, 20).Select(i => new AiPrompt
            {
                Key = i % 3 == 0 ? $"SpecialKey {i}" : $"Key {i}",
                Version = "1.0",
                Text = $"Text {i}"
            }).ToArray();
            db.Set<AiPrompt>().AddRange(items);
            db.SaveChanges();
        }

        [Fact]
        public async void Handler_FiltersByKey()
        {
            var handler = new GetAiPromptListQuery.Handler(_repo);
            var query = new GetAiPromptListQuery { Key = "SpecialKey", Page = 1, PageSize = 100 };

            var result = await handler.Handle(query, CancellationToken.None);

            var expected = DbContext.Set<AiPrompt>().Where(p => p.Key.Contains("SpecialKey")).Select(p => new AiPromptDto { Id = p.Id, Key = p.Key, Version = p.Version, Text = p.Text }).ToList();

            Assert.Equal(expected.Count, result.TotalItems);
            Assert.Equal(expected.Count, result.Items.Count);
        }

        [Fact]
        public async void Handler_ReturnsAll_WhenNoFilter()
        {
            var handler = new GetAiPromptListQuery.Handler(_repo);
            var query = new GetAiPromptListQuery { Page = 1, PageSize = 100 };

            var result = await handler.Handle(query, CancellationToken.None);

            var total = DbContext.Set<AiPrompt>().Count();
            Assert.Equal(total, result.TotalItems);
            Assert.Equal(total, result.Items.Count);
        }
    }
}
