using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Sai.DealAssistant.WebApi.Authorizations
{
	public static class CorsPolicies
	{
		public const string AllowFrontend = "AllowFrontend";

		public static CorsPolicy AllowFrontendCorsPolicy(string origins)
		{
			var originsArr = origins.Split(";", StringSplitOptions.RemoveEmptyEntries);

			return new CorsPolicyBuilder()
				.WithOrigins(originsArr.ToArray())
				.AllowAnyHeader()
				.AllowAnyMethod()
				.AllowCredentials()
				.Build();
		}
	}
}
