using Sai.DealAssistant.Domain.Http;

namespace Sai.DealAssistant.Infrastructure.Http;

public class ExternalPageReader : IExternalPageReader
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ExternalPageReader(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> ReadPageRaw(string url)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
