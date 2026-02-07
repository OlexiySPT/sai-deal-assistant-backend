using AutoMapper;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;

public class DealListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = default!;
    public string State { get; set; } = default!;
    public string? Description { get; set; } = string.Empty;
    public string? Industry { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public decimal? ProposalAmount { get; set; }
    public string? CurrencyCode { get; set; }
    public string? AmountType { get; set; }

    public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Deal, DealListItemDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type!.Type))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State!.State))
                .ForMember(dest => dest.AmountType, opt => opt.MapFrom(src => src.AmountType != null ? src.AmountType.Type : null));
		}
	}
}
