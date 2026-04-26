using AutoMapper;
using AutoMapper;
using FluentValidation;
using Sai.DealAssistant.Domain;
using MediatR;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Deals.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Application.Entities.Deals.Commands;

	public class CreateDealCommand : DealDto, IRequest<DealDto>
	{
		public string? FirmName { get; set; }
    public class Validator : AbstractValidator<CreateDealCommand>
		{
			private readonly IEnumCache<DealState> _dealStateCache;
        private readonly IEnumCache<DealType> _dealTypeCache;

        public Validator(IEnumCache<DealState> dealStateCache, IEnumCache<DealType> dealTypeCache)
			{

            _dealStateCache = dealStateCache;
            _dealTypeCache = dealTypeCache;

            RuleFor(c => c.TypeId)
                .MustAsync(async (cmd, typeId, cToken) => (await _dealTypeCache.GetAllAsync()).Any(p => p.Id == typeId))
                .WithMessage($"Incorrect Type Id. It must be one of [{string.Join(", ", _dealTypeCache.GetAllAsync().Result.Select(p => p.Id.ToString()))}]");

            RuleFor(c => c.Name)
                .NotEmpty()
                .NotNull();

            RuleFor(c => c.StateId)
                .MustAsync(async (cmd, stateId, cToken) => (await _dealStateCache.GetAllAsync()).Any(p => p.Id == stateId))
                .WithMessage($"Incorrect State Id. It must be one of [{string.Join(", ", _dealStateCache.GetAllAsync().Result.Select(p => p.Id.ToString()))}]");

            RuleFor(c => c.Url)
                .Must(url => string.IsNullOrWhiteSpace(url)
                    || Uri.TryCreate(url, UriKind.Absolute, out var u) && (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps))
                .WithMessage("Url must be empty or a valid absolute http(s) URL.");
        }
		}

        public class Handler : IRequestHandler<CreateDealCommand, DealDto>
		{
			private readonly ICrudRepository<Deal> _repository;
			private readonly IMapper _mapper;
			private readonly IFullFirmRepository _fullFirmRepository;
			private readonly ICrudRepository<Firm> _firmRepository;
			private readonly IUnitOfWork _unitOfWork;

			public Handler(
				ICrudRepository<Deal> repository,
				IMapper mapper,
				IFullFirmRepository fullFirmRepository,
				ICrudRepository<Firm> firmRepository,
				IUnitOfWork unitOfWork)
			{
				_repository = repository;
				_mapper = mapper;
                _fullFirmRepository = fullFirmRepository;
				_firmRepository = firmRepository;
				_unitOfWork = unitOfWork;
			}

			public async Task<DealDto> Handle(CreateDealCommand request, CancellationToken cancellationToken)
			{
				Deal? createdDeal = null;

				await _unitOfWork.ExecuteResilientTransactionAsync(async () =>
				{
					// If FirmId < 1, require FirmName
					if (request.FirmId < 1)
					{
						if (string.IsNullOrWhiteSpace(request.FirmName))
						{
							throw new BadRequestExceptionOverride("FirmId or FirmName must be provided. If FirmId is not set, FirmName is required to create or resolve the firm.");
						}

						var firm = await _fullFirmRepository.FirstOrDefaultAsync(f => f.Name.ToLower() == request.FirmName.ToLower());
						if (firm != null)
						{
							request.FirmId = firm.Id;
						}
						else
						{
							var newFirm = new Firm
							{
								Name = request.FirmName,
								Country = "Unknown"
							};
							var createdFirm = await _firmRepository.CreateAsync(newFirm);
							request.FirmId = createdFirm.Id;
						}
					}

					var newDeal = _mapper.Map<Deal>(request);
					createdDeal = await _repository.CreateAsync(newDeal);

					if (createdDeal == null)
					{
						throw new NotFoundExceptionOverride(nameof(Deal), request.Name);
					}
				}, cancellationToken);

				return _mapper.Map<DealDto>(createdDeal!);
			}
		}
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<CreateDealCommand, Deal>();
		}
	}
}

