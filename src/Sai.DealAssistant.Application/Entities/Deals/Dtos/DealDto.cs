using AutoMapper;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;

public class DealDto
{
    public DateOnly? StartDate { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Company { get; set; } = null!;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? AiSearchInfo { get; set; }
    public string? AiBriefDescription { get; set; }
    public string? Industry { get; set; }
    public string? Status { get; set; }
    public int TypeId { get; set; }
    public int StateId { get; set; }
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
			CreateMap<Deal, DealDto>()
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company))
                .ForMember(dest => dest.AmountTypeId, opt => opt.MapFrom(src => src.AmountTypeId))
                .ReverseMap();
		}
	}
}
