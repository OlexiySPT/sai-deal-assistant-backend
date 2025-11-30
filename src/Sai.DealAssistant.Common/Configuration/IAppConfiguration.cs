namespace Sai.DealAssistant.Common.Configuration
{
    public interface IAppConfiguration
    {
        string AppConnectionString { get; }
        string MigrationConnectionString { get; }
    }
}
