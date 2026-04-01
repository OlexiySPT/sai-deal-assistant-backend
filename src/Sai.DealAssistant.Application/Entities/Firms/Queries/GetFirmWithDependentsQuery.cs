using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Firms.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories;

namespace Sai.DealAssistant.Application.Entities.Firms.Queries;

public class GetFirmWithDependentsQuery : IRequest<FirmWithDependenciesDto>
{
    public GetFirmWithDependentsQuery(int id)
    {
        Id = id;
    }

    public int Id { get; }

    public class Handler : IRequestHandler<GetFirmWithDependentsQuery, FirmWithDependenciesDto>
    {
        private readonly IFullFirmRepository _repository;
        private readonly IMapper _mapper;

        public Handler(IFullFirmRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<FirmWithDependenciesDto> Handle(GetFirmWithDependentsQuery request, CancellationToken cancellationToken)
        {
            var firm = await _repository.FirstOrDefaultAsync(f => f.Id == request.Id);

            if (firm == null)
            {
                throw new NotFoundExceptionOverride(nameof(Firm), request.Id);
            }

            return _mapper.Map<FirmWithDependenciesDto>(firm);
        }
    }
}

