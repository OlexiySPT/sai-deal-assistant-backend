using AutoMapper;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.Entities.Firms.Dtos;

public class FirmListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Firm, FirmListItemDto>();
        }
    }
}