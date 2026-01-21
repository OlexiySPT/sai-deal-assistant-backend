using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.DealTags.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.ContactPersons.Commands;

public class DeleteDealTagCommand : IRequest<DealTagDto>
{
    public DeleteDealTagCommand(int id) => Id = id;
    public int Id { get; set; }

    public class Handler : IRequestHandler<DeleteDealTagCommand, DealTagDto>
    {
        private readonly ICrudRepository<DealTag> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<DealTag> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<DealTagDto> Handle(DeleteDealTagCommand request, CancellationToken cancellationToken)
        {
            DealTag? deleted = await _repository.DeleteAsync(request.Id);

            if (deleted == null)
            {
                throw new NotFoundExceptionOverride(nameof(DealTag), request.Id);
            }

            return _mapper.Map<DealTagDto>(deleted);
        }
    }
}