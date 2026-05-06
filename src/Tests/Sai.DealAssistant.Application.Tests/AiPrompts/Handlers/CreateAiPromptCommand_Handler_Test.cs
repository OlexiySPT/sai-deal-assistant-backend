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
    public class CreateAiPromptCommand_Handler_Test : UnitTestBase
    {
        private readonly CrudRepository<AppDbContext, AiPrompt> _repo;

        public CreateAiPromptCommand_Handler_Test()
            : base(seedTestData: false)
        {
            _repo = new CrudRepository<AppDbContext, AiPrompt>(DbContext);
        }

        [Fact]
        public async void Handler_CreatesPrompt()
        {
            var handler = new CreateAiPromptCommand.Handler(_repo, Mapper);
            var cmd = new CreateAiPromptCommand { Key = "k1", Version = "1.0", Text = "text" };

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Id > 0);

            var fromDb = DbContext.Set<AiPrompt>().Single(p => p.Id == result.Id);
            Assert.Equal("k1", fromDb.Key);
            Assert.Equal("1.0", fromDb.Version);
            Assert.Equal("text", fromDb.Text);
        }
    }
}
