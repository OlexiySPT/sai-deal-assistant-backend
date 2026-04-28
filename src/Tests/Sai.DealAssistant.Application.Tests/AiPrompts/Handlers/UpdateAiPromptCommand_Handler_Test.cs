using AutoMapper;
using Sai.DealAssistant.Application.Entities.AiPrompts.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System.Linq;
using System.Threading;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.AiPrompts.Handlers
{
    public class UpdateAiPromptCommand_Handler_Test : UnitTestBase
    {
        private readonly CrudRepository<AppDbContext, AiPrompt> _repo;

        public UpdateAiPromptCommand_Handler_Test()
            : base(seedTestData: false)
        {
            _repo = new CrudRepository<AppDbContext, AiPrompt>(DbContext);

            // seed one
            using var db = CreateNewDbContext();
            db.Set<AiPrompt>().Add(new AiPrompt { Key = "k1", Version = "1.0", Text = "old" });
            db.SaveChanges();
        }

        [Fact]
        public async void Handler_UpdatesPrompt()
        {
            var handler = new UpdateAiPromptCommand.Handler(_repo, Mapper);
            var existing = DbContext.Set<AiPrompt>().First();

            var cmd = new UpdateAiPromptCommand { Id = existing.Id, Key = existing.Key, Version = "1.1", Text = "new text" };

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(existing.Id, result.Id);
            Assert.Equal("1.1", result.Version);
            Assert.Equal("new text", result.Text);

            var fromDb = DbContext.Set<AiPrompt>().Single(p => p.Id == existing.Id);
            Assert.Equal("1.1", fromDb.Version);
            Assert.Equal("new text", fromDb.Text);
        }
    }
}
