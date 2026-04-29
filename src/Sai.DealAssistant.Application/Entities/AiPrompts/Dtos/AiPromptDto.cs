using AutoMapper;
using Sai.DealAssistant.Application.Entities.AiPrompts.Commands;
using Sai.DealAssistant.Domain.Entities;
using System;

namespace Sai.DealAssistant.Application.Entities.AiPrompts.Dtos;

public class AiPromptDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AiPromptDto, AiPrompt>().ReverseMap();
    }
}