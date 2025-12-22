using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Domain.Entities.ReadOnly;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories.Generic;

namespace Sai.DealAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnumsController : ControllerBase
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IDictionary<string, Type> _enumTypes;

	public EnumsController(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

		// Discover all concrete types that implement IEnum and derive from BaseReadOnlyEntity
		var domainAssembly = typeof(BaseReadOnlyEntity).Assembly;
		_enumTypes = domainAssembly
			.GetTypes()
			.Where(t => t.IsClass && !t.IsAbstract
				&& typeof(IEnum).IsAssignableFrom(t)
				&& typeof(BaseReadOnlyEntity).IsAssignableFrom(t))
			.ToDictionary(t => t.Name.ToLowerInvariant(), t => t);
	}

	// GET api/enums
	[HttpGet]
	public IActionResult GetAvailable()
	{
		return Ok(_enumTypes.Keys.OrderBy(k => k));
	}

	// GET api/enums/{enumName}
	[HttpGet("{enumName}")]
	public async Task<IActionResult> Get(string enumName, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(enumName))
			return BadRequest();

		if (!_enumTypes.TryGetValue(enumName.ToLowerInvariant(), out var entityType))
			return NotFound(new { message = $"Enum type '{enumName}' not found." });

		using var scope = _serviceProvider.CreateScope();
		var provider = scope.ServiceProvider;

		// Try to resolve IEnumCache<TEntity>
		var cacheServiceType = typeof(IEnumCache<>).MakeGenericType(entityType);
		var cacheService = provider.GetService(cacheServiceType);

		if (cacheService is null)
		{
			throw new InvalidOperationException($"IEnumCache<{entityType.Name}> service is not registered.");
        }
        // use dynamic to call GetAllAsync on resolved generic IEnumCache<TEntity>
        var cachedItems = await ((dynamic)cacheService).GetAllAsync();
        return Ok(cachedItems);
    }
}