using Microsoft.Extensions.Configuration;

namespace Sai.DealAssistant.Common.Configuration;

public class AppConfigurationFromConfigJson : IAppConfiguration
{
    private readonly IConfigurationRoot _configuration;
    public AppConfigurationFromConfigJson()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json") // This requires the Microsoft.Extensions.Configuration.Json package and using directive
            .Build();
    }

    public int EnumTablesCacheExpitrationMins
    {
        get => GetIntConfigValue("EnumTablesCacheExpirationMins", 10);
    }
    public int DefaultResultPageSize
    {
        get => GetIntConfigValue("DefaultResultPageSize", 10);
    }

    string IAppConfiguration.AppConnectionString => _configuration.GetConnectionString("AppConnection")!;

    string IAppConfiguration.MigrationConnectionString => _configuration.GetConnectionString("MigrationConnection")!;

    string IAppConfiguration.AllowedCorsOrigins => _configuration["AllowedCorsOrigins"] ?? string.Empty;

    private int GetIntConfigValue(string name, int defaultValue)
    {
        string str = _configuration[name] ?? "";
        if (int.TryParse(str, out int result))
        {
            return result;
        }
        return defaultValue;
    }
}
