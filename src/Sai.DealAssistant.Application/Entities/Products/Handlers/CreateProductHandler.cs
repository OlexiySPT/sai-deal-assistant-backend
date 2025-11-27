using MediatR;
using Sai.DealAssistant.Application.Entities.Products.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories;

namespace Sai.DealAssistant.Application.Entities.Products.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _repository;

    public CreateProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(request.Name, request.Price);
        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();
        return product.Id;
    }
}