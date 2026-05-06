using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.AiMetadatas.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.AiMetadatas.Queries;

public class GetAiMetadataQuery : IRequest<AiMetadataDto>
{
    public GetAiMetadataQuery(int id)
    {
        Id = id;
    }

    public int Id { get; set; }

    public class Handler : IRequestHandler<GetAiMetadataQuery, AiMetadataDto>
    {
        private readonly IReadRepository<AiMetadata> _repository;
        private readonly IMapper _mapper;

        public Handler(IReadRepository<AiMetadata> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AiMetadataDto> Handle(GetAiMetadataQuery request, CancellationToken cancellationToken)
        {
            var item = await _repository.FirstOrDefaultAsync(f => f.Id == request.Id);

            if (item == null)
            {
                throw new NotFoundExceptionOverride(nameof(AiMetadata), request.Id);
            }

            return _mapper.Map<AiMetadataDto>(item);
        }
    }
}
