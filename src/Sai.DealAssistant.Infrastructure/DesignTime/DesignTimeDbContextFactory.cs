using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Sai.DealAssistant.Common.Configuration;
using Sai.DealAssistant.Infrastructure.Persistence;

namespace Sai.DealAssistant.Infrastructure.DesignTime;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private IMyConfiguration _configuration;
    public DesignTimeDbContextFactory(IMyConfiguration configuration)
    {
        _configuration = configuration;
    }
    public AppDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseNpgsql(_configuration.AppConnectionString);

        return new AppDbContext(builder.Options);
    }
}
