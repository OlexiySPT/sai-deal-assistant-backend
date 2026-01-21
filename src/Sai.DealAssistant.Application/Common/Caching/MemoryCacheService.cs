using Microsoft.Extensions.Caching.Memory;

namespace Sai.DealAssistant.Application.Common.Caching;

public class MemoryCacheService : IMemoryCacheService
{
	private readonly IMemoryCache _cache;

	public MemoryCacheService(IMemoryCache cache)
	{
		_cache = cache;
	}

	public bool TryGetValue<T>(string key, out T? value)
		where T : class
	{
		if (_cache.TryGetValue(key, out T? result))
		{
			value = result;
			return true;
		}

		value = default;
		return false;
	}

	public void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
	{
		ICacheEntry entry = _cache.CreateEntry(key);
		entry.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
		entry.Value = value;
		entry.Dispose();
	}
}
