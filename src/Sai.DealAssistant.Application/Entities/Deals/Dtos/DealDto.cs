using AutoMapper;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly;
using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;

public class DealDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? AiSearchInfo { get; set; }
    public string? AiBriefDescription { get; set; }
    public string? Industry { get; set; }
    public string? Status { get; set; }
    public int TypeId { get; set; }
    public int StateId { get; set; }

    public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Deal, DealDto>().ReverseMap();
		}
	}
}
