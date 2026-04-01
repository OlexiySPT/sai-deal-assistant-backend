using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Firms.Queries;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Repositories;
using SAI.DealAssistant.TestUtils.Unit;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Firms.Handlers.Queries;

public class GetFirmWithDependentsQuery_Handler_Test : UnitTestBase
{
    private readonly FullFirmRepository _firmRepository;

    public GetFirmWithDependentsQuery_Handler_Test()
        : base(seedTestData: true)
    {
        _firmRepository = new FullFirmRepository(DbContext);
    }

    [Fact]
    public async Task Handler_ReturnsFirmWithDependenciesDto_WhenFirmExists()
    {
        // Arrange
        var handler = new GetFirmWithDependentsQuery.Handler(_firmRepository, Mapper);

        var expectedFirm = DbContext.Firms
            .Include(f => f.ContactPersons)
            .AsNoTracking()
            .OrderBy(f => f.Id)
            .First();

        var query = new GetFirmWithDependentsQuery(expectedFirm.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedFirm.Id, result.Id);
        Assert.Equal(expectedFirm.Name, result.Name);
        Assert.Equal(expectedFirm.Country, result.Country);
        Assert.Equal(expectedFirm.Description, result.Description);
        Assert.Equal(expectedFirm.ContactPersons.Count, result.ContactPersons.Count);
    }

    [Fact]
    public async Task Handler_IncludesContactPersons_WhenFirmHasContactPersons()
    {
        // Arrange
        var handler = new GetFirmWithDependentsQuery.Handler(_firmRepository, Mapper);

        var firmWithContacts = DbContext.Firms
            .Include(f => f.ContactPersons)
            .AsNoTracking()
            .FirstOrDefault(f => f.ContactPersons.Any());

        if (firmWithContacts == null)
        {
            using var db = CreateNewDbContext();
            var firm = new Firm { Name = "Test Firm With Contacts", Country = "TestLand" };
            db.Firms.Add(firm);
            db.SaveChanges();

            var cp = new ContactPerson { FirmId = firm.Id, Name = "Test Contact", Position = "Manager" };
            db.ContactPersons.Add(cp);
            db.SaveChanges();

            firmWithContacts = DbContext.Firms
                .Include(f => f.ContactPersons)
                .AsNoTracking()
                .First(f => f.Id == firm.Id);
        }

        var query = new GetFirmWithDependentsQuery(firmWithContacts.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.ContactPersons);
        Assert.Equal(firmWithContacts.ContactPersons.Count, result.ContactPersons.Count);

        var expectedContactIds = firmWithContacts.ContactPersons.Select(c => c.Id).OrderBy(id => id).ToList();
        var resultContactIds = result.ContactPersons.Select(c => c.Id).OrderBy(id => id).ToList();
        Assert.Equal(expectedContactIds, resultContactIds);
    }

    [Fact]
    public async Task Handler_ThrowsNotFoundException_WhenFirmDoesNotExist()
    {
        // Arrange
        var handler = new GetFirmWithDependentsQuery.Handler(_firmRepository, Mapper);
        var nonExistentId = -9999;
        var query = new GetFirmWithDependentsQuery(nonExistentId);

        // Act / Assert
        await Assert.ThrowsAsync<NotFoundExceptionOverride>(() =>
            handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handler_ReturnsFirmWithEmptyContactPersons_WhenNoContactPersonsExist()
    {
        // Arrange
        var handler = new GetFirmWithDependentsQuery.Handler(_firmRepository, Mapper);

        using var db = CreateNewDbContext();
        var firm = new Firm { Name = "Firm Without Contacts " + Guid.NewGuid(), Country = "TestLand" };
        db.Firms.Add(firm);
        db.SaveChanges();

        var query = new GetFirmWithDependentsQuery(firm.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(firm.Id, result.Id);
        Assert.Empty(result.ContactPersons);
    }
}
