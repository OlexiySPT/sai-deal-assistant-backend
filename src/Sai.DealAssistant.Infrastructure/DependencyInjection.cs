using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Sai.DealAssistant.Common.Configuration;
using Sai.DealAssistant.Domain;
using Sai.DealAssistant.Domain.AI;
using Sai.DealAssistant.Domain.AI.Repositories;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Http;
using Sai.DealAssistant.Domain.Repositories;
using Sai.DealAssistant.Domain.Repositories.Generic;
using Sai.DealAssistant.Infrastructure.AI;
using Sai.DealAssistant.Infrastructure.AI.Repositories;
using Sai.DealAssistant.Infrastructure.Http;
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

        services.AddGenericRepositories(configuration);
        services.AddFieldUpdateRepositories(configuration);
        services.AddSpecificRepositories(configuration);
        services.AddScoped<IUnitOfWork, UnitOfWork>(sp =>
        {
            var dbContext = sp.GetRequiredService<AppDbContext>();
            return new UnitOfWork(dbContext);
        });

        services.AddHttpClient();
        // Register Playwright and Browser as singletons for reuse
        services.AddSingleton<IPlaywright>(_ => Playwright.CreateAsync().GetAwaiter().GetResult());
        services.AddSingleton<IBrowser>(sp =>
        {
            var playwright = sp.GetRequiredService<IPlaywright>();
            return playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true }).GetAwaiter().GetResult();
        });

        // Register AiClient as IAiClient using HttpClientFactory
        services.AddHttpClient<IAiClient, AiClient>();
        return services;
    }

    private static IServiceCollection AddSpecificRepositories(this IServiceCollection services, IAppConfiguration configuration)
    {
        // Do not forget to add new specific repos here
        services.AddScoped<IFullDealRepository, FullDealRepository>();
        services.AddScoped<IFullFirmRepository, FullFirmRepository>();
        services.AddScoped<ISeedRepository, SeedRepository>(sp =>
            new SeedRepository(
                sp.GetRequiredService<ILogger<SeedRepository>>(),
                sp.GetRequiredService<AppDbContext>(),
                configuration));

        // Register ExternalPageScrapper and ExternalPageReader
        services.AddScoped<IExternalPageScrapper, ExternalPageScrapper>();
        services.AddScoped<IExternalPageReader, ExternalPageReader>();
        // Register AiResultRepository
        services.AddScoped<IAiResultRepository, AiResultRepository>();
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

    private static IServiceCollection AddFieldUpdateRepositories(this IServiceCollection services, IAppConfiguration configuration)
    {
        services.AddScoped<IFieldUpdateRepository<string>>(sp => new FieldUpdateRepository<AppDbContext, string>(sp.GetRequiredService<AppDbContext>()));
        services.AddScoped<IFieldUpdateRepository<decimal?>>(sp => new FieldUpdateRepository<AppDbContext, decimal?>(sp.GetRequiredService<AppDbContext>()));
        services.AddScoped<IFieldUpdateRepository<DateTimeOffset?>>(sp => new FieldUpdateRepository<AppDbContext, DateTimeOffset?>(sp.GetRequiredService<AppDbContext>()));
        services.AddScoped<IFieldUpdateRepository<DateOnly?>>(sp => new FieldUpdateRepository<AppDbContext, DateOnly?>(sp.GetRequiredService<AppDbContext>()));
        services.AddScoped<IMultiFieldUpdateRepository>(sp => new MultiFieldUpdateRepository<AppDbContext>(sp.GetRequiredService<AppDbContext>()));
        return services;
    }
}