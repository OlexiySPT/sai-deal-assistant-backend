using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Entities.SampleEmployees.Dtos;
using Sai.DealAssistant.Common.Enums;
using Sai.DealAssistant.Domain.Entities.Samples;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Linq.Expressions;

namespace Sai.DealAssistant.Application.Entities.SampleEmployees.Queries
{
	public class GetSampleEmployeesQuery : PaginatedQueryRequest<QueryResult<SampleEmployeePreviewDto>>
	{
		public GetSampleEmployeesQuery()
			: base()
		{
		}

		public int? CustomerId { get; set; }

		public string? FullName { get; set; }

		public string? Email { get; set; }

		public class Handler : IRequestHandler<GetSampleEmployeesQuery, QueryResult<SampleEmployeePreviewDto>>
		{
			private readonly IReadRepository<SampleEmployee> _repository;
			private readonly IMapper _mapper;

			public Handler(IReadRepository<SampleEmployee> repository, IMapper mapper)
			{
				_repository = repository;
				_mapper = mapper;
			}

			public async Task<QueryResult<SampleEmployeePreviewDto>> Handle(
				GetSampleEmployeesQuery request, CancellationToken cancellationToken)
			{
				IQueryable<SampleEmployee> qry = _repository.GetAll();
				if (request.CustomerId != null && request.CustomerId > 0)
				{
					qry = qry.Where(p => p.CustomerId == request.CustomerId);
				}

				if (!string.IsNullOrWhiteSpace(request.FullName))
				{
					qry = qry.Where(p => p.FullName.Contains(request.FullName));
				}

				if (!string.IsNullOrWhiteSpace(request.Email))
				{
					qry = qry.Where(p => p.Email != null && p.Email!.Contains(request.Email));
				}

				int totalItems = await _repository.CountAsync(qry);
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
				IReadOnlyCollection<SampleEmployeePreviewDto> result = await _repository.SelectPageAsync(
					qry,
					// AutoMapper works, but we had better to avoid it, because it selects all columns including ones, we don`t need in the DTO
					// and cause unnecessary data selection and storage on the client(early pessimization)
					// And since we mustn`t use one DTO for different methods, we had better to map columns right here
					// And we have bonus: this keeps all code together and makes it straightforward
					// p => _mapper.Map<SampleEmployeePreviewDto>(p),
					p => new SampleEmployeePreviewDto
					{
						Id = p.Id,
						CustomerId = p.CustomerId,
						FullName = p.FullName,
						Email = p.Email
					},
					request.Page,
					request.PageSize,
					request.SortBy,
					request.SortDirection == SortDirections.Desc,
					new Dictionary<string, Expression<Func<SampleEmployee, object>>>
					{
						{ "id", c => c.Id },
						{ "fullname", c => c.FullName },
						{ "email", c => c.Email! }
					});
#pragma warning restore SA1515 // Single-line comment should be preceded by blank line

				return new QueryResult<SampleEmployeePreviewDto>(
					result, totalItems);
			}
		}
	}
}
