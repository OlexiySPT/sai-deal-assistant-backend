using AutoMapper;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.Entities.Events.Dtos
{

    public class EventDto
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public int Pos { get; set; } = 0;
        public string? Agenda { get; set; }
        public string? Result { get; set; }
        public int? ContactPersonId { get; set; }
        public int TypeId { get; set; }
        public int StateId { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Event, EventDto>().ReverseMap();
            }
        }
    }
}