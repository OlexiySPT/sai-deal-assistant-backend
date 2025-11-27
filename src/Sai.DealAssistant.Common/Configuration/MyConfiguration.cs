using Microsoft.Extensions.Configuration;

namespace Sai.DealAssistant.Common.Configuration;

public class MyConfiguration : IMyConfiguration
{
    private readonly IConfigurationRoot _configuration;
    public MyConfiguration()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json") // This requires the Microsoft.Extensions.Configuration.Json package and using directive
            .Build();
    }

    string IMyConfiguration.AppConnectionString => _configuration.GetConnectionString("AppConnection")!;

    string IMyConfiguration.MigrationConnectionString => _configuration.GetConnectionString("MigrationConnection")!;

}
