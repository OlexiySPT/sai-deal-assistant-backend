using Hrx.Benefits.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;

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
