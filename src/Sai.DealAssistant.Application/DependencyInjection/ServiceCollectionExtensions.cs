using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Sai.DealAssistant.Application.Commands;
using Sai.DealAssistant.Application.Handlers;
using Services.Application.Queries;

namespace Sai.DealAssistant.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register MediatR handlers from this assembly
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductHandler).Assembly));

            // Register FluentValidation validators located in this assembly (if any)
             services.AddValidatorsFromAssembly(typeof(CreateProductHandler).Assembly);

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
}