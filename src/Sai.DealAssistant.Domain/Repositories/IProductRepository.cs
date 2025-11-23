using System;
using System.Threading.Tasks;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Domain.Repositories
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task<Product?> GetByIdAsync(Guid id);
        Task SaveChangesAsync();
    }
}