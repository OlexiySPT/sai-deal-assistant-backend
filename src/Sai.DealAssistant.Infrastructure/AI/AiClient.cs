using Sai.DealAssistant.Common.Configuration;
using Sai.DealAssistant.Domain.AI;
using Sai.DealAssistant.Domain.AI.Repositories;
using Sai.DealAssistant.Domain.Entities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Sai.DealAssistant.Infrastructure.AI;

public class AiClient : IAiClient
{
    private readonly HttpClient _httpClient;
    private readonly IAppConfiguration _config;
    private readonly IAiResultRepository _aiResultRepository;
    private readonly IAiPromptRepository _aiPromptRepository;

    public AiClient(HttpClient httpClient, IAppConfiguration config, IAiResultRepository aiResultRepository)
    {
        _httpClient = httpClient;
        _config = config;
        _aiResultRepository = aiResultRepository;
    }

    // Added overload with prompt repository injection
    public AiClient(HttpClient httpClient, IAppConfiguration config, IAiResultRepository aiResultRepository, IAiPromptRepository aiPromptRepository)
        : this(httpClient, config, aiResultRepository)
    {
        _aiPromptRepository = aiPromptRepository;
    }

    public async Task<string> Chat(
        AiTaskTypesEnum taskType,
        string prompt,
        int? dealId = null,
        TimeSpan? timeout = null)
    {
        // Base address from configuration
        if (!string.IsNullOrWhiteSpace(_config.AiApiBaseUrl))
        {
            _httpClient.BaseAddress = new Uri(_config.AiApiBaseUrl);
        }

        // Path/url for the API endpoint (e.g. "/api/chat" or full path)
        var address = _config.AiApiUrl;
        if (string.IsNullOrWhiteSpace(address))
        {
            address = "/api/chat";
        }

        // Select model names from configuration
        string model;
        switch (taskType)
        {
            case AiTaskTypesEnum.Fast:
                model = _config.AiApiFastModelName;
                break;
            case AiTaskTypesEnum.Balanced:
                model = _config.AiApiBalancedModelName;
                break;
            case AiTaskTypesEnum.Complex:
                model = _config.AiApiComplexModelName;
                break;
            default:
                model = _config.AiApiComplexModelName;
                break;
        }

        var payload = new
        {
            model,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            stream = false
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        _httpClient.Timeout = timeout ?? TimeSpan.FromSeconds(600);

        _httpClient.DefaultRequestHeaders.Authorization = null;
        if (!string.IsNullOrWhiteSpace(_config.AiApiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.AiApiKey);
        }

        // Log the request
        var aiRequest = new AiRequest
        {
            Type = "ai_chat",
            Model = model,
            Prompt = prompt,
            CreatedAt = DateTime.UtcNow,
            DealId = dealId
        };
        var requestId = await _aiResultRepository.AddRequestAsync(aiRequest);

        var start = DateTime.UtcNow;
        var response = await _httpClient.PostAsync(address, content);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var duration = (DateTime.UtcNow - start).TotalSeconds;

        // Log the result
        var aiResult = new AiResult
        {
            RequestId = requestId,
            Result = responseString,
            DurationSeconds = duration,
            Success = true,
            CreatedAt = DateTime.UtcNow
        };
        await _aiResultRepository.AddResultAsync(aiResult);

        return responseString;
    }
}
