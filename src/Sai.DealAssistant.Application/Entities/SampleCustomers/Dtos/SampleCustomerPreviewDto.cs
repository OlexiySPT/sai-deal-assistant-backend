using AutoMapper;
using Sai.DealAssistant.Domain.Entities.Samples;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;

public class SampleCustomerPreviewDto
{
	public SampleCustomerPreviewDto(int id, string code, string name)
	{
		Id = id;
		Code = code;
		Name = name;
	}

	public int Id { get; }

	public string Code { get; }

	public string Name { get; }

	public string FullName
	{
		get { return $"({Code}) {Name}"; }
	}

	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<SampleCustomer, SampleCustomerPreviewDto>()
				.ConstructUsing(c => new SampleCustomerPreviewDto(c.Id, c.Code, c.Name));
		}
	}
}
