using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.DealContactReps.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.DealContactReps.Commands;

public class DeleteDealContactRepCommand : IRequest<DealContactRepDto>
{
    public DeleteDealContactRepCommand(int id) => Id = id;
    public int Id { get; set; }

    public class Handler : IRequestHandler<DeleteDealContactRepCommand, DealContactRepDto>
    {
        private readonly ICrudRepository<ContactPerson> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<ContactPerson> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<DealContactRepDto> Handle(DeleteDealContactRepCommand request, CancellationToken cancellationToken)
        {
            ContactPerson? deleted = await _repository.DeleteAsync(request.Id);

            if (deleted == null)
            {
                throw new NotFoundExceptionOverride(nameof(ContactPerson), request.Id);
            }

            return _mapper.Map<DealContactRepDto>(deleted);
        }
    }
}