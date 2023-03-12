using AzureOpenAISample.Marv.Models;

namespace AzureOpenAISample.Marv.Implementations;

public interface IOpenAIService
{
    Task<OpenAIResponse> GetResponseAsync(string prompt, string modelName);
}