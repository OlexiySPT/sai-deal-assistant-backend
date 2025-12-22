using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Sai.DealAssistant.Application.Common.Behaviors;
using Sai.DealAssistant.Application.Common.Caching;
using Sai.DealAssistant.Application.Entities.Deals.Commands;
using Sai.DealAssistant.Application.System.Seeding;

namespace Sai.DealAssistant.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddScoped<IMemoryCache, MemoryCache>();
        services.AddScoped<IMemoryCacheService, MemoryCacheService>();
        // Register MediatR handlers from this assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateDealCommand).Assembly));

        // Register FluentValidation validators located in this assembly (if any)
         services.AddValidatorsFromAssembly(typeof(CreateDealCommand).Assembly);

        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}