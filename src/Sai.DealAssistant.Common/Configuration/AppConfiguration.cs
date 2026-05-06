using Microsoft.Extensions.Configuration;

namespace Sai.DealAssistant.Common.Configuration;

public class AppConfigurationFromConfigJson : IAppConfiguration
{
    private readonly IConfigurationRoot _configuration;
    
    public AppConfigurationFromConfigJson()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        
        // Load .env file in Development environment
        if (environment == "Development")
        {
            var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
            if (File.Exists(envFilePath))
            {
                DotNetEnv.Env.Load(envFilePath);
            }
        }
        
        _configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
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

    bool IAppConfiguration.SeedTestData => GetBoolConfigValue("SeedTestData", false);

    int IAppConfiguration.MultiplyDealsTargetRowCount => GetIntConfigValue("MultiplyDealsTargetRowCount", 0);

    int IAppConfiguration.BulkSqlTimeoutSeconds => GetIntConfigValue("BulkSqlTimeoutSeconds", 900);

    public string AiApiBaseUrl => _configuration["AiApiBaseUrl"] ?? string.Empty;

    // New properties implemented
    public string AiApiUrl => _configuration["AiApiUrl"] ?? string.Empty;
    public string AiApiKey => _configuration["AiApiKey"] ?? string.Empty;
    public string AiApiFastModelName => _configuration["AiApiFastModelName"] ?? string.Empty;
    public string AiApiBalancedModelName => _configuration["AiApiBalancedModelName"] ?? string.Empty;
    public string AiApiComplexModelName => _configuration["AiApiComplexModelName"] ?? string.Empty;

    private int GetIntConfigValue(string name, int defaultValue)
    {
        string str = _configuration[name] ?? "";
        if (int.TryParse(str, out int result))
        {
            return result;
        }
        return defaultValue;
    }
    private bool GetBoolConfigValue(string name, bool defaultValue)
    {
        string str = _configuration[name] ?? "";
        if (bool.TryParse(str, out bool result))
        {
            return result;
        }
        return defaultValue;
    }
}
