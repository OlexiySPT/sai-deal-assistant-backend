using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Sai.DealAssistant.Domain.Entities.Samples;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using Sai.DealAssistant.Domain.Exceptions;
using SAI.DealAssistant.TestUtils.Unit.GenericRepositoryTests.Persistance;

namespace Sai.DealAssistant.Infrastructure.Tests.Repositories.GenericRepositoryTests
{
    public class FieldUpdateRepositoryTests
    {
        private readonly GenericRepoTestDbContext _dbContext;
        private readonly FieldUpdateRepository<GenericRepoTestDbContext, string> _stringRepo;
        private readonly FieldUpdateRepository<GenericRepoTestDbContext, decimal?> _numericRepo;
        private readonly FieldUpdateRepository<GenericRepoTestDbContext, DateTime?> _dateRepo;

        public FieldUpdateRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<GenericRepoTestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new GenericRepoTestDbContext(options);

            // Seed sample customer and employee
            var customer = new SampleCustomer { Name = "Test Customer" };
            _dbContext.SampleCustomers.Add(customer);
            _dbContext.SaveChanges();

            var employee = new SampleEmployee
            {
                FirstName = "John",
                LastName = "Doe",
                CustomerId = customer.Id
            };
            _dbContext.SampleEmployees.Add(employee);
            _dbContext.SaveChanges();

            _stringRepo = new FieldUpdateRepository<GenericRepoTestDbContext, string>(_dbContext);
            _numericRepo = new FieldUpdateRepository<GenericRepoTestDbContext, decimal?>(_dbContext);
            _dateRepo = new FieldUpdateRepository<GenericRepoTestDbContext, DateTime?>(_dbContext);
        }

        [Fact]
        public async Task UpdateFieldAsync_UpdatesStringField()
        {
            var employee = await _dbContext.SampleEmployees.FirstAsync();
            var newName = "Jane";

            await _stringRepo.UpdateFieldAsync("SampleEmployee", "FirstName", employee.Id, newName);

           var employeeAfterUpdate = await _dbContext.SampleEmployees.FindAsync(employee.Id);
            
            Assert.Equal(newName, employeeAfterUpdate.FirstName);
        }

        [Fact]
        public async Task UpdateFieldAsync_UpdatesNumericField()
        {
            var employee = await _dbContext.SampleEmployees.FirstAsync();
            var newSalary = 12345.67m;

            await _numericRepo.UpdateFieldAsync("SampleEmployee", "Salary", employee.Id, newSalary);

            var employeeAfterUpdate = await _dbContext.SampleEmployees.FindAsync(employee.Id);
            Assert.Equal(newSalary, employeeAfterUpdate.Salary);
        }

        [Fact]
        public async Task UpdateFieldAsync_UpdatesDateField()
        {
            var employee = await _dbContext.SampleEmployees.FirstAsync();
            var newDate = DateTime.UtcNow;

            await _dateRepo.UpdateFieldAsync("SampleEmployee", "HireDate", employee.Id, newDate);

            var updated = await _dbContext.SampleEmployees.FindAsync(employee.Id);
            Assert.Equal(newDate, updated.HireDate);
        }

        [Fact]
        public async Task UpdateFieldAsync_TableNotExists_Throws()
        {
            await Assert.ThrowsAsync<TableNotExistsException>(
                () => _stringRepo.UpdateFieldAsync("NonExistingEntity", "FirstName", 1, "Test"));
        }

        [Fact]
        public async Task UpdateFieldAsync_ColumnNotExists_Throws()
        {
            var employee = await _dbContext.SampleEmployees.FirstAsync();
            await Assert.ThrowsAsync<ColumnNotExistsException>(
                () => _stringRepo.UpdateFieldAsync("SampleEmployee", "NonExistingField", employee.Id, "Test"));
        }

        [Fact]
        public async Task UpdateFieldAsync_IdNotExists_Throws()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _stringRepo.UpdateFieldAsync("SampleEmployee", "FirstName", -999, "Test"));
        }
    }
}