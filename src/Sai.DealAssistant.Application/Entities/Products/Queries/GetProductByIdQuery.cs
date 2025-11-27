using Sai.DealAssistant.Domain.Entities;
using MediatR;

namespace Sai.DealAssistant.Application.Entities.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<Product?>;