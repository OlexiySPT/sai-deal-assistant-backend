using AutoMapper;
using AutoMapper;
using Sai.DealAssistant.Application.Entities.ContactPersons.Dto;
using Sai.DealAssistant.Domain.Entities;
using System.Collections.ObjectModel;

namespace Sai.DealAssistant.Application.Entities.Firms.Dtos;

public class FirmWithDependenciesDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<ContactPersonListItemDto> ContactPersons { get; set; }
        = new Collection<ContactPersonListItemDto>();

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Firm, FirmWithDependenciesDto>();
        }
    }
}