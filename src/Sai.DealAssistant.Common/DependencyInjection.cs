using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sai.DealAssistant.Application.DealAutomation;
using Sai.DealAssistant.Common.Configuration;
using Sai.DealAssistant.Common.JobQueue;
using Sai.DealAssistant.Common.Queue;

namespace Sai.DealAssistant.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCommon(this IServiceCollection services)
    {
        services.AddSingleton<IAppConfiguration, AppConfigurationFromConfigJson>();

        services.AddSingleton<IJobQueue<IJobQueueCommand>, InMemoryJobQueue<IJobQueueCommand>>();
        services.AddHostedService<JobBackgroundService>();
        return services;
    }

}