using MediatR;
using Sai.DealAssistant.Application.Entities.ContactPersons.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.ContactPersons.Queries;

/// <summary>
/// Returns all contact persons for the given firm.
/// Sorting and filtering will be done on the FE.
/// </summary>
public class GetFirmContactPersonsQuery : IRequest<QueryResult<ContactPersonListItemDto>>
{
    public GetFirmContactPersonsQuery() { }

    public int FirmId { get; set; }

    public class Handler : IRequestHandler<GetFirmContactPersonsQuery, QueryResult<ContactPersonListItemDto>>
    {
        private readonly IReadRepository<ContactPerson> _repository;

        public Handler(IReadRepository<ContactPerson> repository)
        {
            _repository = repository;
        }

        public async Task<QueryResult<ContactPersonListItemDto>> Handle(GetFirmContactPersonsQuery request, CancellationToken cancellationToken)
        {
            var qry = _repository.GetAll().Where(p => p.FirmId == request.FirmId).OrderBy(p => p.Name);

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