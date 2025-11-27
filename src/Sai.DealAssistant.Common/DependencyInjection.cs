using Microsoft.Extensions.DependencyInjection;
using Sai.DealAssistant.Common.Configuration;

namespace Sai.DealAssistant.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCommon(this IServiceCollection services)
    {
        services.AddSingleton<IMyConfiguration, MyConfiguration>();
        return services;
    }

}