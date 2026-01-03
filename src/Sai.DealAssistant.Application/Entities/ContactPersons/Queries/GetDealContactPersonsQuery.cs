using MediatR;
using Sai.DealAssistant.Application.Entities.ContactPersons.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.ContactPersons.Queries;

/// <summary>
/// We assume this queries all contact reps for the deal
/// Sorting and filtering will be done on the FE
public class GetDealContactPersonsQuery : IRequest<QueryResult<ContactPersonListItemDto>>
{
    public GetDealContactPersonsQuery() { }

    public int DealId { get; set; }

    public class Handler : IRequestHandler<GetDealContactPersonsQuery, QueryResult<ContactPersonListItemDto>>
    {
        private readonly IReadRepository<ContactPerson> _repository;

        public Handler(IReadRepository<ContactPerson> repository)
        {
            _repository = repository;
        }

        public async Task<QueryResult<ContactPersonListItemDto>> Handle(GetDealContactPersonsQuery request, CancellationToken cancellationToken)
        {
            var qry = _repository.GetAll().Where(p => p.DealId == request.DealId).OrderBy(p => p.Name);

            var totalItems = await _repository.CountAsync(qry);

            var result = await _repository.SelectAsync(
                qry,
                p => new ContactPersonListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Position = p.Position,
                    Email = p.Email
                }
            );

            return new QueryResult<ContactPersonListItemDto>(result, totalItems);
        }
    }
}