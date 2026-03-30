namespace Sai.DealAssistant.Common.Configuration
{
    public interface IAppConfiguration
    {
        string AppConnectionString { get; }
        string MigrationConnectionString { get; }
        string AllowedCorsOrigins { get; }
        int EnumTablesCacheExpitrationMins { get; }
        int DefaultResultPageSize { get; }
        bool SeedTestData { get; }
    }
}
