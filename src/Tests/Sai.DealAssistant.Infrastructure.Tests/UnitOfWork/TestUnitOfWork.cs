using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Infrastructure.UnitOfWork;

namespace Sai.DealAssistant.Infrastructure.Tests.UnitOfWork
{
	public class TestUnitOfWork : BaseUnitOfWork
	{
		public TestUnitOfWork(DbContext context)
			: base(context)
		{
		}
	}
}
