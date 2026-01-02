using MediatR;
using Sai.DealAssistant.Application.Entities.Events.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Events.Queries;

/// <summary>
/// We assume this queries all events for the deal
/// Sorting and filtering will be done on the FE 
public class GetDealEventsQuery : IRequest<QueryResult<EventListItemDto>>
{
    public GetDealEventsQuery() : base() { }

    public int DealId { get; set; }

    public class Handler : IRequestHandler<GetDealEventsQuery, QueryResult<EventListItemDto>>
    {
        private readonly IReadRepository<Event> _repository;

        public Handler(IReadRepository<Event> repository)
        {
            _repository = repository;
        }

        public async Task<QueryResult<EventListItemDto>> Handle(GetDealEventsQuery request, CancellationToken cancellationToken)
        {
            var qry = _repository.GetAll().Where(p => p.DealId == request.DealId).OrderByDescending(p => p.Id);

            var totalItems = await _repository.CountAsync(qry);

            var result = await _repository.SelectAsync(
                qry,
                p => new EventListItemDto
                {
                    Id = p.Id,
                    Date = p.Date,
                    Pos = p.Pos,
                    Agenda = p.Agenda,
                    Result = p.Result,
                    State = p.State.State,
                    Type = p.Type.Name
                }
            );

            return new QueryResult<EventListItemDto>(result, totalItems);
        }
    }
}