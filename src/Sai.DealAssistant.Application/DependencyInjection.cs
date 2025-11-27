using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Sai.DealAssistant.Application.Common.Behaviors;
using Sai.DealAssistant.Application.Common.Caching;
using Sai.DealAssistant.Application.Entities.Products.Commands;
using Sai.DealAssistant.Application.Entities.Products.Handlers;
using Sai.DealAssistant.Application.Entities.Products.Queries;
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
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductHandler).Assembly));

        // Register FluentValidation validators located in this assembly (if any)
         services.AddValidatorsFromAssembly(typeof(CreateProductHandler).Assembly);

        services.AddScoped<DatabaseSeeder>();

        return services;
    }

    public static IEndpointRouteBuilder MapApplicationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // POST /products  -> Create product
        endpoints.MapPost("/products", async (IMediator mediator, CreateProductCommand command) =>
        {
            var id = await mediator.Send(command);
            return Results.Created($"/products/{id}", id);
        });

        // GET /products/{id} -> Get product by id
        endpoints.MapGet("/products/{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var product = await mediator.Send(new GetProductByIdQuery(id));
            return product is not null ? Results.Ok(product) : Results.NotFound();
        });

        return endpoints;
    }
}