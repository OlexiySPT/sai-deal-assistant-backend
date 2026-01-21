using AutoMapper;
using Sai.DealAssistant.Application.Entities.EventNotes.Dto;
using Sai.DealAssistant.Domain.Entities;
using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Application.Entities.Events.Dtos;

public class EventWithDependenciesListItemDto
{
    public int Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public string Type { get; set; } = default!;
    public string? ContactPerson { get; set; }
    public string State { get; set; } = default!;
    public string? Agenda { get; set; }
    public string? Result { get; set; }
    public int Pos { get; set; }
    public ICollection<EventNoteDto> Notes { get; set; }
        = new Collection<EventNoteDto>();

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Event, EventWithDependenciesListItemDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type!.Name))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State!.State))
                .ForMember(dest => dest.ContactPerson, opt => opt.MapFrom(src => (src.ContactPerson == null ? null : $"{src.ContactPerson!.Name}, {src.ContactPerson!.Position}")));
        }
    }
}