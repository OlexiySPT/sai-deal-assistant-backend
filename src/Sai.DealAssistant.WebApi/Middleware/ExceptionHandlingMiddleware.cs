using Sai.DealAssistant.Application.Common.Exceptions;
using System.Text.Json;
using AppValidationException = Sai.DealAssistant.Application.Common.Exceptions.ValidationExceptionOverride;

namespace Sai.DealAssistant.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;
	private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

	public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			await HandleExceptionAsync(context, ex);
		}
	}

	private async Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		if (context.Response.HasStarted)
		{
			_logger.LogWarning("The response has already started, the exception handling middleware will not modify the response.");
			throw exception;
		}

		context.Response.ContentType = "application/json";

		// NotFound -> 404
		if (exception is NotFoundExceptionOverride nf)
		{
			_logger.LogInformation(nf, "NotFound: {Message}", nf.Message);
			context.Response.StatusCode = StatusCodes.Status404NotFound;
			var payload404 = new { error = "NotFound", message = nf.Message };
			await context.Response.WriteAsync(JsonSerializer.Serialize(payload404, _jsonOptions));
			return;
		}

		// Authorization -> 403
		if (exception is AuthorizationExceptionOverride authEx)
		{
			_logger.LogWarning(authEx, "Authorization failure: {Message}", authEx.Message);
			context.Response.StatusCode = StatusCodes.Status403Forbidden;
			var payload403 = new { error = "Forbidden", message = authEx.Message };
			await context.Response.WriteAsync(JsonSerializer.Serialize(payload403, _jsonOptions));
			return;
		}

		// BadRequest -> 400
		if (exception is BadRequestExceptionOverride badReq)
		{
			_logger.LogWarning(badReq, "Bad request: {Message}", badReq.Message);
			context.Response.StatusCode = StatusCodes.Status400BadRequest;
			var payload400 = new { error = "BadRequest", message = badReq.Message };
			await context.Response.WriteAsync(JsonSerializer.Serialize(payload400, _jsonOptions));
			return;
		}

		// Application ValidationException -> 400 with validation details
		if (exception is AppValidationException appValEx)
		{
			_logger.LogWarning(appValEx, "Validation failed: {Message}", appValEx.Message);
			context.Response.StatusCode = StatusCodes.Status400BadRequest;
			var payloadValidation = new
			{
				error = "ValidationFailed",
				message = appValEx.Message,
				failures = appValEx.Failures
			};
			await context.Response.WriteAsync(JsonSerializer.Serialize(payloadValidation, _jsonOptions));
			return;
		}

		// Generic unhandled exception -> 500
		_logger.LogError(exception, "Unhandled exception");
		context.Response.StatusCode = StatusCodes.Status500InternalServerError;
		var payload500 = new { error = "ServerError", message = "An unexpected error occurred." };
		await context.Response.WriteAsync(JsonSerializer.Serialize(payload500, _jsonOptions));
	}
}