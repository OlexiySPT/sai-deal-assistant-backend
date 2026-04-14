using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Firms.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Firms.Queries;

public class GetFirmQuery : IRequest<FirmDto>
{
    public GetFirmQuery(int id)
    {
        Id = id;
    }

    public int Id { get; set; }

    public class Handler : IRequestHandler<GetFirmQuery, FirmDto>
    {
        private readonly IReadRepository<Firm> _repository;
        private readonly IMapper _mapper;

        public Handler(IReadRepository<Firm> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<FirmDto> Handle(GetFirmQuery request, CancellationToken cancellationToken)
        {
            var firm = await _repository.FirstOrDefaultAsync(f => f.Id == request.Id);

            if (firm == null)
            {
                throw new NotFoundExceptionOverride(nameof(Firm), request.Id);
            }

            return _mapper.Map<FirmDto>(firm);
        }
    }
}