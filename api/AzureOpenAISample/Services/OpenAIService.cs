using AzureOpenAISample.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AzureOpenAISample.Implementations;
public class OpenAIService : IOpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(IConfiguration configuration, ILoggerFactory loggerFactory, HttpClient httpClient)
    {
        _logger = loggerFactory.CreateLogger<OpenAIService>();
        _httpClient = httpClient;
    }

    public async Task<OpenAIResponse> GetResponseAsync(string prompt, string modelName)
    {
        var request = BuildHttpRequestMessage(prompt, modelName);
        var response = await _httpClient.SendAsync(request);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Call returned a non-successful Status Code.");
            throw;
        }

        var data = await response.Content.ReadFromJsonAsync<OpenAIResponse>();
        if (data == null)
        {
            var ex = new Exception($"No response returned or response doesnt match the {nameof(OpenAIResponse)} deserialization model.");
            _logger.LogCritical(ex, ex.Message);
            throw ex;
        }

        if (data.Choices == null || !data.Choices.Any())
        {
            var ex = new Exception($"No {nameof(data.Choices)} returned.");
            _logger.LogCritical(ex, ex.Message);
            throw ex;
        }
        return data;
    }

    private HttpRequestMessage BuildHttpRequestMessage(string prompt, string modelName)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{modelName}/completions?api-version=2022-12-01");
        var openAIRequest = new OpenAIRequest
        {
            Prompt = prompt
        };
        request.Content = new StringContent(JsonSerializer.Serialize(openAIRequest), Encoding.UTF8, "application/json");

        return request;
    }
}
