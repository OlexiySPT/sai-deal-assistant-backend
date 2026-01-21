using Microsoft.AspNetCore.Builder;
using Sai.DealAssistant.WebApi.Middleware;

namespace Sai.DealAssistant.WebApi.Extensions;

public static class ApplicationBuilderExtensions
{
	public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
	{
		return app.UseMiddleware<ExceptionHandlingMiddleware>();
	}
}