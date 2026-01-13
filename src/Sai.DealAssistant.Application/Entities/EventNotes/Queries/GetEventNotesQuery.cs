// Sai.DealAssistant.Application\Entities\EventNotes\Queries\GetEventNotesQuery.cs
using MediatR;
using Sai.DealAssistant.Application.Entities.EventNotes.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.EventNotes.Queries;

/// <summary>
/// Query to return all event notes for a specific event.
/// Sorting and filtering are expected to be handled on the client.
/// </summary>
public class GetEventNotesQuery : IRequest<QueryResult<EventNoteListItemDto>>
{
    public int EventId { get; set; }

    public class Handler : IRequestHandler<GetEventNotesQuery, QueryResult<EventNoteListItemDto>>
    {
        private readonly IReadRepository<EventNote> _repository;

        public Handler(IReadRepository<EventNote> repository)
        {
            _repository = repository;
        }

        public async Task<QueryResult<EventNoteListItemDto>> Handle(GetEventNotesQuery request, CancellationToken cancellationToken)
        {
            var qry = _repository.GetAll().Where(n => n.EventId == request.EventId).OrderBy(n => n.Order);

            var totalItems = await _repository.CountAsync(qry);

            var result = await _repository.SelectAsync(
                qry,
                n => new EventNoteListItemDto
                {
                    Id = n.Id,
                    Order = n.Order,
                    Text = n.Text
                }
            );

            return new QueryResult<EventNoteListItemDto>(result, totalItems);
        }
    }
}