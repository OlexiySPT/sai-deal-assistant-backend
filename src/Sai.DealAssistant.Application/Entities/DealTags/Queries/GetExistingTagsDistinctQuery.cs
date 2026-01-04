using MediatR;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.DealTags.Queries;

public class GetExistingTagsDistinctQuery : IRequest<IEnumerable<string>>
{
    public int DealId { get; set; }

    public class Handler : IRequestHandler<GetExistingTagsDistinctQuery, IEnumerable<string>>
    {
        private readonly IReadRepository<DealTag> _repository;

        public Handler(IReadRepository<DealTag> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<string>> Handle(GetExistingTagsDistinctQuery request, CancellationToken cancellationToken)
        {
            var qry = _repository.GetAll().Where(p => p.DealId == request.DealId).OrderBy(p => p.Tag);

            var result = await _repository.SelectAsync(
                qry,
                p => p.Tag
            );

            return result;
        }
    }
}