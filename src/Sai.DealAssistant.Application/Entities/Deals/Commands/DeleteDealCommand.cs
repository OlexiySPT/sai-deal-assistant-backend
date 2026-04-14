using AutoMapper;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Deals.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Deals.Commands
{
	public class DeleteDealCommand : IRequest<DealDto>
	{
		public DeleteDealCommand(int id)
		{
			Id = id;
		}

		public int Id { get; set; }

		public class Handler : IRequestHandler<DeleteDealCommand, DealDto>
		{
			private readonly ICrudRepository<Deal> _repository;
			private readonly IMapper _mapper;

			public Handler(
				ICrudRepository<Deal> customerRepository,
				IMapper mapper)
			{
				_repository = customerRepository;
				_mapper = mapper;
			}

			public async Task<DealDto> Handle(DeleteDealCommand request, CancellationToken cancellationToken)
			{
                Deal? deletedEntity = await _repository.DeleteAsync(request.Id);

				if (deletedEntity == null)
				{
					throw new NotFoundExceptionOverride(nameof(Deal), request.Id);
				}

				return _mapper.Map<DealDto>(deletedEntity);
			}
		}
	}
}
