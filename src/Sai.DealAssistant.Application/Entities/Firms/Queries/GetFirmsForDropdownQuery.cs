using MediatR;
using Sai.DealAssistant.Application.Common.Expressions;
using Sai.DealAssistant.Application.Entities.Firms.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Firms.Queries;

public class GetFirmsForDropdownQuery : PagedQueryRequest<QueryResult<FirmForDropdownDto>>
{
    public string? Name { get; set; }

    public class Handler : IRequestHandler<GetFirmsForDropdownQuery, QueryResult<FirmForDropdownDto>>
    {
        private readonly IReadRepository<Firm> _repository;

        public Handler(IReadRepository<Firm> repository)
        {
            _repository = repository;
        }

        public async Task<QueryResult<FirmForDropdownDto>> Handle(
            GetFirmsForDropdownQuery request, CancellationToken cancellationToken)
        {
            var qry = _repository.GetAll();

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                qry = qry.Where(StringSearchExpressions.CaseInsensitiveStartsWith<Firm>(x => x.Name, request.Name));
            }

            var totalItems = await _repository.CountAsync(qry);

            var result = await _repository.SelectPageAsync(
                qry,
                p => new FirmForDropdownDto
                {
                    Id = p.Id,
                    Name = p.Name,
                },
                request.Page,
                request.PageSize,
                nameof(Firm.Name).ToLowerInvariant(),
                false
            );

            return new PagedQueryResult<FirmForDropdownDto>(result, totalItems, request.PageSize, request.Page);
        }
    }
}
