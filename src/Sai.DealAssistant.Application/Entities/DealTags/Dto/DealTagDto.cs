using AutoMapper;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.Entities.DealTags.Dto;

public class DealTagDto
{
    public int Id { get; set; }
    public string Tag { get; set; }
    public int DealId { get; set; }
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DealTag, DealTagDto>().ReverseMap();
        }
    }
}
