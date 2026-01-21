using MediatR;
using Sai.DealAssistant.Application.Entities.DealTags.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.DealTags.Queries;

public class GetDealTagsQuery : IRequest<IEnumerable<DealTagDto>>
{
    public int DealId { get; set; }

    public class Handler : IRequestHandler<GetDealTagsQuery, IEnumerable<DealTagDto>>
    {
        private readonly IReadRepository<DealTag> _repository;

        public Handler(IReadRepository<DealTag> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DealTagDto>> Handle(GetDealTagsQuery request, CancellationToken cancellationToken)
        {
            var qry = _repository.GetAll().Where(p => p.DealId == request.DealId).OrderBy(p => p.Tag);

            var result = await _repository.SelectAsync(
                qry,
                p => new DealTagDto
                {
                    Id = p.Id,
                    Tag = p.Tag,
                    DealId = p.DealId
                }
            );

            return result ?? Enumerable.Empty<DealTagDto>();
        }
    }
}