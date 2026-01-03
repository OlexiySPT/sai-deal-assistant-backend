using MediatR;
using Sai.DealAssistant.Application;
using Sai.DealAssistant.Application.Entities.DealContactReps.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.DealContactReps.Queries;

/// <summary>
/// We assume this queries all contact reps for the deal
/// Sorting and filtering will be done on the FE
public class GetDealContactRepsQuery : IRequest<QueryResult<DealContactRepListItemDto>>
{
    public GetDealContactRepsQuery() { }

    public int DealId { get; set; }

    public class Handler : IRequestHandler<GetDealContactRepsQuery, QueryResult<DealContactRepListItemDto>>
    {
        private readonly IReadRepository<ContactPerson> _repository;

        public Handler(IReadRepository<ContactPerson> repository)
        {
            _repository = repository;
        }

        public async Task<QueryResult<DealContactRepListItemDto>> Handle(GetDealContactRepsQuery request, CancellationToken cancellationToken)
        {
            var qry = _repository.GetAll().Where(p => p.DealId == request.DealId).OrderBy(p => p.Name);

            var totalItems = await _repository.CountAsync(qry);

            var result = await _repository.SelectAsync(
                qry,
                p => new DealContactRepListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Position = p.Position,
                    Email = p.Email
                }
            );

            return new QueryResult<DealContactRepListItemDto>(result, totalItems);
        }
    }
}