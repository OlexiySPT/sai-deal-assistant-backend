using AutoMapper;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;

public class SampleCustomerForAccountingDto
{
	public int Id { get; set; }

	public string Code { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	#region TaxInfo
	public string? TaxNumber { get; set; }

	public DateTime? DateRegistered { get; set; }
	#endregion

	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<SampleCustomer, SampleCustomerDto>();
		}
	}
}
