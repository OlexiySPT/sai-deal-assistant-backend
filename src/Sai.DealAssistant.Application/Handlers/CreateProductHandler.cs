using System;
using System.Threading;
using System.Threading.Tasks;
using Sai.DealAssistant.Domain.Entities;
using MediatR;
using Sai.DealAssistant.Domain.Repositories;
using Sai.DealAssistant.Application.Commands;

namespace Sai.DealAssistant.Application.Handlers
{
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
}