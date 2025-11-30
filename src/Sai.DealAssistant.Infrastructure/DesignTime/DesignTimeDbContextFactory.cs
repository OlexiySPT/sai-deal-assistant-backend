using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Sai.DealAssistant.Common.Configuration;
using Sai.DealAssistant.Infrastructure.Persistence;

namespace Sai.DealAssistant.Infrastructure.DesignTime;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private IAppConfiguration _configuration;

    //To make it possible run for add-migration
    public DesignTimeDbContextFactory()
    {
        _configuration = new AppConfiguration();
    }
    public DesignTimeDbContextFactory(IAppConfiguration configuration)
    {
        _configuration = configuration;
    }
    public AppDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseNpgsql(_configuration.MigrationConnectionString);

        return new AppDbContext(builder.Options);
    }
}
