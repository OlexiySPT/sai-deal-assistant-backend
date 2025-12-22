using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sai.DealAssistant.Common.Configuration;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using System.Diagnostics;

namespace Sai.DealAssistant.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IAppConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(
            options => options
                .UseNpgsql(configuration.AppConnectionString)
#if DEBUG
                .EnableSensitiveDataLogging()
                .LogTo(msg => Debug.WriteLine(msg), LogLevel.Information)
#endif
            );

        services.AddGenericRepositories();
        services.AddSpecificRepositories();

        return services;
    }

    private static IServiceCollection AddSpecificRepositories(this IServiceCollection services)
    {
        // Donot forget to add new specific repos here
        services.AddScoped<ISeedRepository, SeedRepository>();
        services.AddScoped<ISampleCustomerRepository, SampleCustomerRepository>();
        return services;
    }

    private static IServiceCollection AddGenericRepositories(this IServiceCollection services)
    {
        Type baseEntityType = typeof(BaseEntity);
        IEnumerable<Type> entityTypes =
            baseEntityType.Assembly.GetTypes()
            .Where(t => t != baseEntityType && baseEntityType.IsAssignableFrom(t))
            .ToArray();
        foreach (Type it in entityTypes)
        {
            services.AddScoped(
                typeof(IReadRepository<>).MakeGenericType(it),
                typeof(ReadRepository<>).MakeGenericType(it));
            services.AddScoped(
                typeof(IEnumCache<>).MakeGenericType(it),
                typeof(EnumCache<>).MakeGenericType(it));
            services.AddScoped(
                typeof(ICrudRepository<>).MakeGenericType(it),
                typeof(CrudRepository<>).MakeGenericType(it));
        }

        return services;
    }

}