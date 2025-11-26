using MediatR;
using Sai.DealAssistant.Application.System.Seeding;

namespace Sai.DealAssistant.Application.System.Commands
{
	public class SeedDatabaseCommand : IRequest
	{
		public SeedDatabaseCommand(bool isDevelopment)
		{
			IsDevelopment = isDevelopment;
		}

		private bool IsDevelopment { get; }

		public class Handler : IRequestHandler<SeedDatabaseCommand>
		{
			private readonly DatabaseSeeder _seeder;

			public Handler(DatabaseSeeder seeder)
			{
				_seeder = seeder;
			}

			public async Task Handle(SeedDatabaseCommand request, CancellationToken cancellationToken)
			{
				await _seeder.SeedAsync();
				if (request.IsDevelopment)
				{
					await _seeder.SeedTestDataAsync();
				}
			}
		}
	}
}
