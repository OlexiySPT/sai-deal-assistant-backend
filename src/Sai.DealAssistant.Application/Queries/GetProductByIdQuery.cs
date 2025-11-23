using Sai.DealAssistant.Domain.Entities;
using MediatR;

namespace Services.Application.Queries
{
    public record GetProductByIdQuery(Guid Id) : IRequest<Product?>;
}