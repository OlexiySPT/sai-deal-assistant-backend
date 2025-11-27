using MediatR;
using Sai.DealAssistant.Application.Entities.Products.Queries;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories;

namespace Sai.DealAssistant.Application.Entities.Products.Handlers;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Product?>
{
    private readonly IProductRepository _repository;

    public GetProductByIdHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return _repository.GetByIdAsync(request.Id);
    }
}