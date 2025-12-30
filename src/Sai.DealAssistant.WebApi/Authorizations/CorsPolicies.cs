using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Sai.DealAssistant.WebApi.Authorizations
{
	public static class CorsPolicies
	{
		public const string AllowFrontend = "AllowFrontend";

		public static CorsPolicy AllowFrontendCorsPolicy(string origins)
		{
			string[] originsArr = origins.Split(";", StringSplitOptions.RemoveEmptyEntries);
			List<string> preparedOrigins = new List<string>();
			foreach (string origin in originsArr)
			{
				preparedOrigins.Add(origin.Trim());
			}

			return new CorsPolicyBuilder()
				.WithOrigins(preparedOrigins.ToArray())
				.AllowAnyHeader()
				.AllowAnyMethod()
				.Build();
		}
	}
}
