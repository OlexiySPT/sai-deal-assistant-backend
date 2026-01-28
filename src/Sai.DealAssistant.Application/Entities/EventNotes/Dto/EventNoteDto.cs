using AutoMapper;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.Entities.EventNotes.Dto;

public class EventNoteDto
{
    public int Id { get; set; }
    public int Order { get; set; }
    public string Text { get; set; } = null!;
    public int EventId { get; set; }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EventNote, EventNoteDto>().ReverseMap();
        }
    }
}
