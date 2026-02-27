using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Queries;
using Sai.DealAssistant.Common.Enums;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System;
using System.Linq;
using System.Threading;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Deals.Handlers.Queries
{
	public class GetDealListQuery_Handler_Test : UnitTestBase
	{
		private readonly ReadRepository<AppDbContext, Deal> _dealRepository;
		private readonly IMapper _mapper;

		public GetDealListQuery_Handler_Test()
			: base(seedTestData: false)
		{
			_dealRepository = new ReadRepository<AppDbContext, Deal>(DbContext);

			// Seed test data
			using (var db = CreateNewDbContext())
			{
				var s1 = new DealState { State = "Open" };
				var s2 = new DealState { State = "Closed" };
				var t1 = new DealType { Type = "Standard" };
				var t2 = new DealType { Type = "Premium" };
                var at1 = new AmountType { Type = "Per Month" };
                var at2 = new AmountType { Type = "Per Day" };
                db.AddRange(s1, s2, t1, t2, at1, at2);
				db.SaveChanges();

				var deals = Enumerable.Range(1, 30).Select(i => new Deal
				{
					Name = $"Deal {i}",
					Description = i % 3 == 0 ? $"Special desc {i}" : $"Desc {i}",
					Industry = i % 2 == 0 ? "Software" : "Finance",
					StateId = i % 2 == 0 ? s1.Id : s2.Id,
					TypeId = i % 3 == 0 ? t2.Id : t1.Id,
					AmountTypeId = i % 2 == 0 ? at1.Id : at2.Id
				}).ToArray();

				db.Deals.AddRange(deals);
				db.SaveChanges();
			}
		}

		[Fact]
		public async void Handler_FiltersByName()
		{
			// Arrange
			var handler = new GetDealListQuery.Handler(_dealRepository);
			var nameFragment = "Deal 1"; // should match Deal 1, Deal 10, Deal 11, etc.
			int pageSize = 100;
			var query = new GetDealListQuery { Name = nameFragment , Page = 1, PageSize = pageSize};

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			var expected = DbContext.Deals
				.Include(d => d.State)
				.Where(d => d.Name != null && d.Name.Contains(nameFragment))
				.Select(p => new DealListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Industry = p.Industry,
                    State = p.State.State,
                    Type = p.Type.Type,
                    CreatedAt = p.CreatedAt,
                })
				.Take(pageSize)
				.ToList();

            Assert.Equal(expected.Count, result.TotalItems);
			Assert.Equal(expected.Count, result.Items.Count);
			Assert.Equivalent(expected.OrderBy(x => x.Id), result.Items.OrderBy(x => x.Id));
		}

		[Fact]
		public async void Handler_FiltersByDescription()
		{
			// Arrange
			var handler = new GetDealListQuery.Handler(_dealRepository);
			var descFragment = "Special desc";
            int pageSize = 100;
            var query = new GetDealListQuery { Description = descFragment , Page = 1, PageSize = pageSize};

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			var expected = DbContext.Deals
				.Include(d => d.State)
				.Where(d => d.Description != null && d.Description.Contains(descFragment))
				.Select(p => new DealListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Industry = p.Industry,
                    State = p.State.State,
                    Type = p.Type.Type,
                    CreatedAt = p.CreatedAt,
                })
				.Take(pageSize)
                .ToList();

			Assert.Equal(expected.Count, result.TotalItems);
			Assert.Equal(expected.Count, result.Items.Count);
			Assert.Equivalent(expected.OrderBy(x => x.Id), result.Items.OrderBy(x => x.Id));
		}

		[Fact]
		public async void Handler_FiltersByIndustry()
		{
			// Arrange
			var handler = new GetDealListQuery.Handler(_dealRepository);
			var industry = "Software";
            int pageSize = 100;
            var query = new GetDealListQuery { Industry = industry, Page = 1, PageSize = pageSize};

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			var expected = DbContext.Deals
				.Include(d => d.State)
				.Where(d => d.Industry != null && d.Industry.Contains(industry))
				.Select(p => new DealListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Industry = p.Industry,
                    State = p.State.State,
                    Type = p.Type.Type,
                    CreatedAt = p.CreatedAt,
                })
				.ToList();

			Assert.Equal(expected.Count, result.TotalItems);
			Assert.Equal(expected.Count, result.Items.Count);
			Assert.Equivalent(expected.OrderBy(x => x.Id), result.Items.OrderBy(x => x.Id));
		}

		[Fact]
		public async void Handler_SortsByName_Ascending()
		{
			// Arrange
			var handler = new GetDealListQuery.Handler(_dealRepository);
			var query = new GetDealListQuery { SortBy = nameof(Deal.Name), SortDirection = SortDirections.Asc, Page = 1, PageSize = 100 };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			var expected = DbContext.Deals
				.Include(d => d.State)
				.OrderBy(d => d.Name)
				.Select(p => new DealListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Industry = p.Industry,
                    State = p.State.State,
                    Type = p.Type.Type,
                    CreatedAt = p.CreatedAt,
                })
				.ToList();

			Assert.Equal(expected.Count, result.TotalItems);
			Assert.Equal(expected.Select(x => x.Id), result.Items.Select(x => x.Id));
		}

		[Fact]
		public async void Handler_SortsByName_Descending()
		{
			// Arrange
			var handler = new GetDealListQuery.Handler(_dealRepository);
			var query = new GetDealListQuery { SortBy = nameof(Deal.Name), SortDirection = SortDirections.Desc, Page = 1, PageSize = 100 };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			var expected = DbContext.Deals
				.Include(d => d.State)
				.OrderByDescending(d => d.Name)
				.Select(p => new DealListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Industry = p.Industry,
                    State = p.State.State,
                    Type = p.Type.Type,
                    CreatedAt = p.CreatedAt,
                })
				.ToList();

			Assert.Equal(expected.Count, result.TotalItems);
			Assert.Equal(expected.Select(x => x.Id), result.Items.Select(x => x.Id));
		}

		[Fact]
		public async void Handler_SortsByIndustry_Ascending()
		{
			// Arrange
			var handler = new GetDealListQuery.Handler(_dealRepository);
			var query = new GetDealListQuery { SortBy = nameof(Deal.Industry), SortDirection = SortDirections.Asc, Page = 1, PageSize = 100 };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			var expected = DbContext.Deals
				.Include(d => d.State)
				.OrderBy(d => d.Industry)
				.Select(p => new DealListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Industry = p.Industry,
                    State = p.State.State,
                    Type = p.Type.Type,
                    CreatedAt = p.CreatedAt,
                })
				.ToList();

			Assert.Equal(expected.Count, result.TotalItems);
			Assert.Equal(expected.Select(x => x.Id), result.Items.Select(x => x.Id));
		}

		[Fact]
		public async void Handler_SortsByIndustry_Descending()
		{
			// Arrange
			var handler = new GetDealListQuery.Handler(_dealRepository);
			var query = new GetDealListQuery { SortBy = nameof(Deal.Industry), SortDirection = SortDirections.Desc, Page = 1, PageSize = 100 };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			var expected = DbContext.Deals
				.Include(d => d.State)
				.OrderByDescending(d => d.Industry)
				.Select(p => new DealListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Industry = p.Industry,
                    State = p.State.State,
                    Type = p.Type.Type,
                    CreatedAt = p.CreatedAt,
                })
				.ToList();

			Assert.Equal(expected.Count, result.TotalItems);
			Assert.Equal(expected.Select(x => x.Id), result.Items.Select(x => x.Id));
		}

		[Fact]
		public async void Handler_FiltersByStateIds()
		{
			// Arrange
			var handler = new GetDealListQuery.Handler(_dealRepository);

            var states = DbContext.DealStates.Take(2).Select(p=>p.Id).ToArray();
            int pageSize = 100;
            var query = new GetDealListQuery{ StateIds = states, Page = 1, PageSize = pageSize };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			var expected = DbContext.Deals
				.Include(d => d.State)
				.Where(d => states.Contains(d.StateId))
				.Select(p => new DealListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Industry = p.Industry,
                    State = p.State.State,
                    Type = p.Type.Type,
                    CreatedAt = p.CreatedAt,
                })
				.Take(pageSize)
                .ToList();

			Assert.Equal(expected.Count, result.TotalItems);
			Assert.Equal(expected.Count, result.Items.Count);
			Assert.Equivalent(expected.OrderBy(x => x.Id), result.Items.OrderBy(x => x.Id));
		}

		[Fact]
		public async void Handler_PaginatesResults()
		{
			// Arrange
			var handler = new GetDealListQuery.Handler(_dealRepository);

			const int pageSize = 5;
			const int page = 2;
			var query = new GetDealListQuery{ Page = page, PageSize = pageSize};

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert: total items equals seeded count and page contains correct number
			var total = DbContext.Deals.Count();
			Assert.Equal(total, result.TotalItems);
			Assert.Equal(pageSize, result.Items.Count);
		}
	}
}