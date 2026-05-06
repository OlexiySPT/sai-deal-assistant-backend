using System;
using System.Threading.Tasks;
using Xunit;
using SAI.DealAssistant.TestUtils.Unit;
using Sai.DealAssistant.Infrastructure.AI.Repositories;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.AI.Repositories;

namespace Tests.Sai.DealAssistant.Infrastructure.Tests.AI
{
    public class AiPromptRepositoryTests : UnitTestBase
    {
        private readonly AiPromptRepository _repo;

        public AiPromptRepositoryTests() : base(seedTestData: false)
        {
            _repo = new AiPromptRepository(DbContext);
        }

        [Fact]
        public async Task CreateAsync_AddsPrompt()
        {
            var prompt = new AiPrompt { Key = "k1", Version = "1.0", Text = "hello" };
            var id = await _repo.CreateAsync(prompt);
            Assert.True(id > 0);

            var fromDb = await DbContext.Set<AiPrompt>().FindAsync(id);
            Assert.NotNull(fromDb);
            Assert.Equal("k1", fromDb.Key);
            Assert.Equal("1.0", fromDb.Version);
            Assert.Equal("hello", fromDb.Text);
        }

        [Fact]
        public async Task GetTextAsync_ReturnsExactVersion()
        {
            var p1 = new AiPrompt { Key = "kt", Version = "1.0", Text = "one" };
            var p2 = new AiPrompt { Key = "kt", Version = "2.0", Text = "two" };
            await _repo.CreateAsync(p1);
            await _repo.CreateAsync(p2);

            var text = await _repo.GetTextAsync("kt", "1.0");
            Assert.Equal("one", text);
        }

        [Fact]
        public async Task GetTextAsync_ReturnsLatestWhenVersionNull()
        {
            await _repo.CreateAsync(new AiPrompt { Key = "kmax", Version = "1.0", Text = "v1" });
            await _repo.CreateAsync(new AiPrompt { Key = "kmax", Version = "1.2", Text = "v1.2" });
            await _repo.CreateAsync(new AiPrompt { Key = "kmax", Version = "1.10", Text = "v1.10" });

            var text = await _repo.GetTextAsync("kmax", null);
            Assert.Equal("v1.10", text);
        }

        [Fact]
        public async Task GetTextAsync_InvalidVersion_Throws()
        {
            await _repo.CreateAsync(new AiPrompt { Key = "kbad", Version = "1.0", Text = "ok" });

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _repo.GetTextAsync("kbad", "abc");
            });
        }
    }
}
