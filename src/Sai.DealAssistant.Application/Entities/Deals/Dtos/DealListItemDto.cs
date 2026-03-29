using AutoMapper;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;

public class DealListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Status { get; set; }
    public string State { get; set; } = default!;

    public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Deal, DealListItemDto>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State!.State));
		}
	}
}
