using AutoMapper;
using Sai.DealAssistant.Domain.Entities.Samples;

namespace Sai.DealAssistant.Application.Entities.SampleEmployees.Dtos
{
	public class SampleEmployeeDto
	{
		public int Id { get; set; }

		public int CustomerId { get; set; }

		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public string? Email { get; set; }

		public string Client { get; set; } = string.Empty;

		public class MappingProfile : Profile
		{
			public MappingProfile()
			{
				CreateMap<SampleEmployee, SampleEmployeeDto>()
					.ForMember(dest => dest.Client, opt => opt.MapFrom(src => $"({src.Customer.Code}) {src.Customer.Name}"));
			}
		}
	}
}
