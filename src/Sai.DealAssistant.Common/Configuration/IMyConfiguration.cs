namespace Sai.DealAssistant.Common.Configuration
{
    public interface IMyConfiguration
    {
        string AppConnectionString { get; }
        string MigrationConnectionString { get; }
    }
}
