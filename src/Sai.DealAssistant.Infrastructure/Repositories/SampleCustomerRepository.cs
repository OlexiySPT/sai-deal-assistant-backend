using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities.Samples;
using Sai.DealAssistant.Domain.Repositories;
using Sai.DealAssistant.Infrastructure.Persistence;

namespace Sai.DealAssistant.Infrastructure.Repositories
{

    public class SampleCustomerRepository : ISampleCustomerRepository
    {
        private readonly AppDbContext _dbContext;
        //private readonly IMapper _mapper;

        public SampleCustomerRepository(
            AppDbContext dbContext/*,
            IMapper mapper*/)
        {
            _dbContext = dbContext;
            //_mapper = mapper;
        }

        private static ColumnsMap<SampleCustomer> ColumnsMap => new ColumnsMap<SampleCustomer>
        {
            { "id", c => c.Id },
            { "code", c => c.Code },
            { "name", c => c.Name }
        };

        public async Task<IReadOnlyCollection<SampleCustomer>> GetAsync()
        {
            return await GetAllAsync();
        }

        public async Task<SampleCustomer?> GetAsync(string code)
        {
            SampleCustomer? customer = await _dbContext.SampleCustomers
                .Where(c => c.Code == code)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            return customer;
        }

        public async Task<SampleCustomer?> GetAsync(int id)
        {
            SampleCustomer? customer = await _dbContext.SampleCustomers
                .Where(c => c.Id == id)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (customer == null)
            {
                return null;
            }

            return customer;
        }

        public async Task<IReadOnlyCollection<SampleCustomer>> SelectAsync(
            string? sortBy,
            bool sortDescending,
            int page,
            int pageSize,
            string? code = null,
            string? name = null)
        {
            IQueryable<SampleCustomer> query = ApplyFilters(code, name);

            var result = await query
                .ApplyOrdering(sortBy, sortDescending, ColumnsMap!)
                .ApplyPaging(page, pageSize)
                .ToListAsync();

            return result.AsReadOnly();
        }

        public async Task<int> CountAsync(string? code = null, string? name = null)
        {
            IQueryable<SampleCustomer> query = ApplyFilters(code, name);
            return await query.CountAsync();
        }

        public async Task<bool> ExistsAsync(string code)
        {
            IQueryable<SampleCustomer> query = _dbContext.SampleCustomers
            .AsNoTracking()
            .AsQueryable();
            return await query.AnyAsync(p => p.Code == code);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            IQueryable<SampleCustomer> query = _dbContext.SampleCustomers
            .AsNoTracking()
            .AsQueryable();

            return await query.AnyAsync(p => p.Id == id);
        }

        public async Task<SampleCustomer?> CreateAsync(SampleCustomer customer)
        {
            _dbContext.SampleCustomers.Add(customer);
            await _dbContext.SaveChangesAsync();

            SampleCustomer? result = await GetAsync(customer.Id);
            return result;
        }

        public async Task<SampleCustomer?> UpdateAsync(SampleCustomer updatedCustomer)
        {
            if (await _dbContext.SampleCustomers.AnyAsync(c => c.Id == updatedCustomer.Id) != true)
            {
                return null;
            }

            _dbContext.SampleCustomers.Update(updatedCustomer);

            await _dbContext.SaveChangesAsync();

            return await GetAsync(updatedCustomer.Id);
        }

        private IQueryable<SampleCustomer> ApplyFilters(string? code, string? name)
        {
            IQueryable<SampleCustomer> query = _dbContext.SampleCustomers
                .AsNoTracking()
                .AsQueryable();
            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(c => c.Code.StartsWith(code.ToUpperInvariant()));
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }

            return query;
        }

        private async Task<IReadOnlyCollection<SampleCustomer>> GetAllAsync() => await _dbContext.SampleCustomers.AsNoTracking().ToListAsync();
    }
}
