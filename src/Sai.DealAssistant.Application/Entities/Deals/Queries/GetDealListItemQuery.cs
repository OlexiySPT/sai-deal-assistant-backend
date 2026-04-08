using MediatR;
using Sai.DealAssistant.Application.Entities.Deals.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Deals.Queries;

public class GetDealListItemQuery : IRequest<DealListItemDto>
{
    public GetDealListItemQuery(int id)
    {
        Id = id;
    }

    public int Id { get; }

    public class Handler : IRequestHandler<GetDealListItemQuery, DealListItemDto>
	{
		private readonly IReadRepository<Deal> _repository;

		public Handler(IReadRepository<Deal> repository)
		{
			_repository = repository;
		}

		public async Task<DealListItemDto> Handle(
            GetDealListItemQuery request, CancellationToken cancellationToken)
		{
			var result = _repository.GetAll()
				.Select(p=>new DealListItemDto
					{
						Id = p.Id,
						Name = p.Name,
						FirmName = p.DenormFirmName,
						LastActionDate = p.DenormLastActionDate,
						State = p.State.State,
						Status = p.Status
					})
				.FirstOrDefault(p=>p.Id ==request.Id);


			return result;
		}
	}
}
