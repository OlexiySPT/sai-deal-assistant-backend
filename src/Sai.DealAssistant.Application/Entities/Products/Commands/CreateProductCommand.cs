using MediatR;

namespace Sai.DealAssistant.Application.Entities.Products.Commands;

public record CreateProductCommand(string Name, decimal Price) : IRequest<Guid>;