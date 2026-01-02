using AutoMapper;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.Entities.Events.Dtos;

public class EventListItemDto
{
    public int Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public string Type { get; set; } = default!;
    public string State { get; set; } = default!;
    public string? Agenda { get; set; }
    public string? Result { get; set; }
    public int Pos { get; set; }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Event, EventListItemDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type!.Name))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State!.State));
        }
    }
}