using AutoMapper;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Commands;

namespace Sai.DealAssistant.Api.Requests.SampleCustomers
{
	public class UpdateSampleCustomerRequest
	{
		public string Code { get; set; } = string.Empty;

		public string Name { get; set; } = string.Empty;

		#region Contact info
		public string? PostalCode { get; set; }

		public string? AddressLn1 { get; set; }

		public string? AddressLn2 { get; set; }

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
	}

	/// <summary>
	/// Customer creation request mapping to CreateSampleCustomerCommand Class.
	/// </summary>
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<UpdateSampleCustomerRequest, UpdateSampleCustomerCommand>();
		}
	}
}
