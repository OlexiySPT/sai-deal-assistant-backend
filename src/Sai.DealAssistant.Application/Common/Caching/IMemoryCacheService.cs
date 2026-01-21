namespace Sai.DealAssistant.Application.Common.Caching;

public interface IMemoryCacheService
{
	bool TryGetValue<T>(string key, out T? value)
		where T : class;

	void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow);
}
