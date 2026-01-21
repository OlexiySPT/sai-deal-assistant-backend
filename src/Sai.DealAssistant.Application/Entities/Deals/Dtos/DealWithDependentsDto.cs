using AutoMapper;
using Sai.DealAssistant.Application.Entities.ContactPersons.Dto;
using Sai.DealAssistant.Application.Entities.DealTags.Dto;
using Sai.DealAssistant.Application.Entities.Events.Dtos;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;

public class DealWithDependentsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? AiSearchInfo { get; set; }
    public string? AiBriefDescription { get; set; }
    public string? Industry { get; set; }
    public string? Status { get; set; }

    public string Type { get; set; }
    public string State { get; set; }

    public ICollection<ContactPersonListItemDto> ContactPersons { get; set; }
        = new Collection<ContactPersonListItemDto>();

    public ICollection<EventWithDependenciesListItemDto> Events { get; set; }
        = new Collection<EventWithDependenciesListItemDto>();

    public ICollection<DealTagDto> Tags { get; set; }
        = new Collection<DealTagDto>();

    public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Deal, DealWithDependentsDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type!.Type))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State!.State)); ;
		}
	}
}
