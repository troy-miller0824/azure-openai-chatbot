using AzureOpenAISample.Marv.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AzureOpenAISample.Marv.Implementations;
public class OpenAIService : IOpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(IConfiguration configuration, ILoggerFactory loggerFactory, HttpClient httpClient)
    {
        _logger = loggerFactory.CreateLogger<OpenAIService>();
        _httpClient = httpClient;
    }

    public async Task<OpenAIResponse> GetResponseAsync(List<OpenAIRequestMessage> messages, string modelName)
    {
        var request = BuildHttpRequestMessage(messages, modelName);
        var response = await _httpClient.SendAsync(request);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Call returned a non-successful Status Code ({response.StatusCode}) with reason '{response.ReasonPhrase}'.");
            throw;
        }

        var data = await response.Content.ReadFromJsonAsync<OpenAIResponse>();
        if (data == null)
        {
            var ex = new Exception($"No response returned or response doesnt match the {nameof(OpenAIResponse)} deserialization model.");
            _logger.LogCritical(ex, ex.Message);
            throw ex;
        }

        if (data.choices == null || !data.choices.Any())
        {
            var ex = new Exception($"No {nameof(data.choices)} returned.");
            _logger.LogCritical(ex, ex.Message);
        }
        return data;
    }

    private HttpRequestMessage BuildHttpRequestMessage(List<OpenAIRequestMessage> messages, string modelName)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{modelName}/chat/completions?api-version=2024-08-01-preview");
        var openAIRequest = new OpenAIRequest
        {
            Messages = messages
        };
        request.Content = new StringContent(JsonSerializer.Serialize(openAIRequest), Encoding.UTF8, "application/json");

        return request;
    }
}
