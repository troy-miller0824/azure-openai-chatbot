using AzureOpenAISample.Marv.Models;

namespace AzureOpenAISample.Marv.Implementations;

public interface IOpenAIService
{
    Task<OpenAIResponse> GetResponseAsync(List<OpenAIRequestMessage> messages, string modelName);
}