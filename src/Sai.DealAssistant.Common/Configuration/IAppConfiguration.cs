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
        /// <summary>
        /// When > 0, deals (and their dependents) will be multiplied until the Deals table
        /// reaches at least this many rows. 0 disables multiplication.
        /// </summary>
        int MultiplyDealsTargetRowCount { get; }
        /// <summary>
        /// Command timeout in seconds used for bulk SQL multiplication operations.
        /// Defaults to 900 (15 minutes) when not configured.
        /// </summary>
        int BulkSqlTimeoutSeconds { get; }

        string AiApiBaseUrl { get; }
        string AiApiUrl { get; }
        string AiApiKey { get; }
        string AiApiFastModelName { get; }
        string AiApiBalancedModelName { get; }
        string AiApiComplexModelName { get; }
    }
}
