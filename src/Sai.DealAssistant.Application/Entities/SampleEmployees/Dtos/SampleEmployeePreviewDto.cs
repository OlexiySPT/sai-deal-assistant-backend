using AutoMapper;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.Entities.SampleEmployees.Dtos
{
	public class SampleEmployeePreviewDto
	{
		public int Id { get; set; }

		public int CustomerId { get; set; }

		public string? FullName { get; set; }

		public string? Email { get; set; }

		public class MappingProfile : Profile
		{
			public MappingProfile()
			{
				CreateMap<SampleEmployee, SampleEmployeePreviewDto>()
					.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
					.ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
					.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
					.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));
			}
		}
	}
}
