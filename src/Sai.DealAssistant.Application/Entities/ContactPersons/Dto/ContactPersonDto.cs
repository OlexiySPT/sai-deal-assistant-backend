using AutoMapper;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.Entities.ContactPersons.Dto;

public class ContactPersonDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Position { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Description { get; set; }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ContactPerson, ContactPersonDto>().ReverseMap();
        }
    }
}