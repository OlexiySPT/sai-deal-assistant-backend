using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Events.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Events.Commands;

public class DeleteEventCommand : IRequest<EventDto>
{
    public DeleteEventCommand(int id)
    {
        Id = id;
    }

    public int Id { get; set; }

    public class Handler : IRequestHandler<DeleteEventCommand, EventDto>
    {
        private readonly ICrudRepository<Event> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<Event> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EventDto> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            Event? deleted = await _repository.DeleteAsync(request.Id);

            if (deleted == null)
            {
                throw new NotFoundExceptionOverride(nameof(Event), request.Id);
            }

            return _mapper.Map<EventDto>(deleted);
        }
    }
}