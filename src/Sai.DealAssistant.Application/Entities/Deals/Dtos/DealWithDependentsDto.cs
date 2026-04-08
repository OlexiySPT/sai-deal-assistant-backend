using AutoMapper;
using Sai.DealAssistant.Application.Entities.ContactPersons.Dto;
using Sai.DealAssistant.Application.Entities.DealTags.Dto;
using Sai.DealAssistant.Application.Entities.Events.Dtos;
using Sai.DealAssistant.Application.Entities.Firms.Dtos;
using Sai.DealAssistant.Domain.Entities;
using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Application.Entities.Deals.Dtos;

public class DealWithDependentsDto
{
    public DateOnly StartDate { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? InitialLetter { get; set; }

    public string? Url { get; set; }
    public string? AiSearchInfo { get; set; }
    public string? AiBriefDescription { get; set; }
    public string? Industry { get; set; }
    public string? Status { get; set; }
    public int TypeId { get; set; } 
    public int StateId { get; set; }
    public int FirmId { get; set; }

    public DateOnly? DenormDenormLastActionDate { get; set; }

    public FirmWithDependenciesDto Firm { get; set; } = null!;
    public ICollection<EventWithDependenciesListItemDto> Events { get; set; }
        = new Collection<EventWithDependenciesListItemDto>();

    public ICollection<DealTagDto> Tags { get; set; }
        = new Collection<DealTagDto>();

    public decimal? ProposalAmount { get; set; }
    public decimal? MinClientAmount { get; set; }
    public decimal? MaxClientAmount { get; set; }
    public string? CurrencyCode { get; set; }
    public decimal? ExchangeRate { get; set; }
    public int? AmountTypeId { get; set; }
    public string? AmountType { get; set; }

    public class MappingProfile : Profile
	{
		public MappingProfile()
		{
            CreateMap<Deal, DealWithDependentsDto>()
                .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.Type!.Id))
                .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.State!.Id))
                .ForMember(dest => dest.AmountType, opt => opt.MapFrom(src => src.AmountType != null ? src.AmountType.Type : null));
		}
	}
}
