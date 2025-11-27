using MediatR;

namespace Sai.DealAssistant.Application.Commands;

public record CreateProductCommand(string Name, decimal Price) : IRequest<Guid>;