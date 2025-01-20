using System.Text.Json.Serialization;

namespace AzureOpenAISample.Marv.Models;

public class OpenAIRequestMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}