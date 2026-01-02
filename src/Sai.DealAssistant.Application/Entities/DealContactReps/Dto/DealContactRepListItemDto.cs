using AutoMapper;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.Entities.DealContactReps.Dtos;

public class DealContactRepListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Position { get; set; }
    public string? Email { get; set; }


    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DealContactRep, DealContactRepListItemDto>();
        }
    }
}