using AzureOpenAISample.Models;

namespace AzureOpenAISample.Implementations;

public interface IOpenAIService
{
    Task<OpenAIResponse> GetResponseAsync(string prompt);
}