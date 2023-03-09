using AzureOpenAISample.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureOpenAISample.Implementations;
public class OpenAIService : IOpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAIService> _logger;
    private readonly IConfiguration _configuration;


    public OpenAIService(IConfiguration configuration, ILoggerFactory loggerFactory, HttpClient httpClient)
    {
        _logger = loggerFactory.CreateLogger<OpenAIService>();
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<OpenAIResponse> GetResponseAsync(string prompt)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"completions?api-version=2022-12-01");

        var openAIRequest = new OpenAIRequest
        {
            Prompt = prompt
        };
        request.Content = new StringContent(JsonSerializer.Serialize(openAIRequest), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<OpenAIResponse>();
        if (data == null)
        {
            throw new Exception($"No response returned or response doesnt match the {nameof(OpenAIResponse)} deserialization model.");
        }
        if (data.Choices == null || !data.Choices.Any())
        {
            throw new Exception($"No {nameof(data.Choices)} returned.");
        }
        return data;
    }
}
