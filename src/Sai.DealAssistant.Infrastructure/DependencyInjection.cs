using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sai.DealAssistant.Common.Configuration;
using Sai.DealAssistant.Domain;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
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
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddGenericRepositories(configuration);
        services.AddSpecificRepositories(configuration);

        return services;
    }

    private static IServiceCollection AddSpecificRepositories(this IServiceCollection services, IAppConfiguration configuration)
    {
        // Do not forget to add new specific repos here
        services.AddScoped<IFullDealRepository, FullDealRepository>();
        services.AddScoped<ISeedRepository, SeedRepository>();
        return services;
    }

    private static IServiceCollection AddGenericRepositories(this IServiceCollection services, IAppConfiguration configuration)
    {
        Type appDbContextType = typeof(AppDbContext);
        Type baseNonTrackedEntityType = typeof(BaseNonTrackedEntity);
        Type baseEntityType = typeof(BaseEntity);
        Type iEnumType = typeof(IEnum);
        IEnumerable<Type> entityTypes =
            baseNonTrackedEntityType.Assembly.GetTypes()
            .Where(t => !t.IsAbstract && t.IsClass && baseNonTrackedEntityType.IsAssignableFrom(t))
            .ToArray();
        int enumExpirationMinutes = configuration.EnumTablesCacheExpitrationMins;
        foreach (Type it in entityTypes)
        {
            services.AddScoped(typeof(IReadRepository<>).MakeGenericType(it), typeof(ReadRepository<,>).MakeGenericType(appDbContextType, it));
            services.AddScoped(typeof(ICrudRepository<>).MakeGenericType(it), typeof(CrudRepository<,>).MakeGenericType(appDbContextType, it));
            
            if (iEnumType.IsAssignableFrom(it))
            {
                services.AddScoped(
                    typeof(IEnumCache<>).MakeGenericType(it), 
                    sp => ActivatorUtilities.CreateInstance(sp, typeof(EnumCache<>).MakeGenericType(it), enumExpirationMinutes)
                );
            }
        }

        return services;
    }
}