using AutoMapper;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.Entities.AiMetadatas.Dtos;

public class AiMetadataDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AiMetadataDto, AiMetadata>().ReverseMap();
    }
}
