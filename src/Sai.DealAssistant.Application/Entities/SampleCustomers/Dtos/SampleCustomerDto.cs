using AutoMapper;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;

public class SampleCustomerDto
{
	public int Id { get; set; }

	public string Code { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	#region Contact info
	public string? Country { get; set; }

	public string? Phone { get; set; }

	public string? Email { get; set; }
	#endregion

	#region TaxInfo
	public string? TaxNumber { get; set; }

	public string? VatPayerNumber { get; set; }

	public string? SocialSecurityPayerNumber { get; set; }

	public string? TaxPayerScheme { get; set; }

	public DateTime RegistrationDate { get; set; }
	#endregion

	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<SampleCustomer, SampleCustomerDto>();
		}
	}
}
