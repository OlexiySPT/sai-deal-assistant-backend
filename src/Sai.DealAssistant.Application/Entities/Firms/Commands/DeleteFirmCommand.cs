using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Firms.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Firms.Commands;

public class DeleteFirmCommand : IRequest<FirmDto>
{
    public DeleteFirmCommand(int id)
    {
        Id = id;
    }

    public int Id { get; set; }

    public class Handler : IRequestHandler<DeleteFirmCommand, FirmDto>
    {
        private readonly ICrudRepository<Firm> _repository;
        private readonly IMapper _mapper;

        public Handler(ICrudRepository<Firm> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<FirmDto> Handle(DeleteFirmCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _repository.DeleteAsync(request.Id);

            if (deleted == null)
            {
                throw new NotFoundExceptionOverride(nameof(Firm), request.Id);
            }

            return _mapper.Map<FirmDto>(deleted);
        }
    }
}