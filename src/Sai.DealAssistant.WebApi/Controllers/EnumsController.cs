using Microsoft.AspNetCore.Mvc;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.WebApi.Controllers;

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
		var domainAssembly = typeof(BaseNonTrackedEntity).Assembly;
		_enumTypes = domainAssembly
			.GetTypes()
			.Where(t => t.IsClass && !t.IsAbstract
				&& typeof(IEnum).IsAssignableFrom(t)
				&& typeof(BaseNonTrackedEntity).IsAssignableFrom(t))
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

		// Remove navigational/complex properties from response by projecting to dictionaries
		var enumerable = (cachedItems as IEnumerable) ?? throw new InvalidOperationException("Cached items are not enumerable.");
		var shaped = enumerable
			.Cast<object>()
			.Select(item =>
			{
				var dict = new Dictionary<string, object?>();
				var props = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
					.Where(p => IsSimpleType(p.PropertyType));
				foreach (var p in props)
				{
					dict[p.Name] = p.GetValue(item);
				}
				return dict;
			})
			.ToList();

		return Ok(shaped);
	}

	private static bool IsSimpleType(Type type)
	{
		var t = Nullable.GetUnderlyingType(type) ?? type;

		if (t.IsPrimitive) return true;
		if (t.IsEnum) return true;
		if (t == typeof(string)) return true;
		if (t == typeof(decimal)) return true;
		if (t == typeof(DateTime)) return true;
		if (t == typeof(DateTimeOffset)) return true;
		if (t == typeof(TimeSpan)) return true;
		if (t == typeof(Guid)) return true;

		// value types (structs) are considered simple if not complex; keep conservative:
		if (t.IsValueType && !t.IsPrimitive && !t.IsEnum)
			return true;

		return false;
	}
}