using AzureOpenAISample.Marv.Models;
using System.Text.Json.Serialization;

namespace AzureOpenAISample.Marv.Models;


//Marv settings: https://platform.openai.com/examples/default-marv-sarcastic-chat
public class OpenAIRequest
{
    [JsonPropertyName("messages")]
    public List<OpenAIRequestMessage> Messages { get; set; } = new List<OpenAIRequestMessage>();

    /// <summary>
    /// Controls randomness. Lowering the temperature means that the model will produce more repetitive and deterministic responses. 
    /// Increasing the temperature will result in more unexpected or creative responses. Try adjusting temperature or Top P but not both.
    /// </summary>
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = .5d;

    /// <summary>
    /// Similar to temperature, this controls randomness but uses a different method. 
    /// Lowering Top P will narrow the model’s token selection to likelier tokens. 
    /// Increasing Top P will let the model choose from tokens with both high and low likelihood. 
    /// Try adjusting temperature or Top P but not both.
    /// </summary>
    [JsonPropertyName("top_p")]
    public double TopPropabilities { get; set; } = .3d;

    /// <summary>
    /// Reduce the chance of repeating a token proportionally based on how often it has appeared in the text so far. 
    /// This decreases the likelihood of repeating the exact same text in a response.
    /// </summary>
    [JsonPropertyName("frequency_penalty")]
    public double FrequencyPenalty { get; set; } = .5d;

    /// <summary>
    /// Reduce the chance of repeating any token that has appeared in the text at all so far. 
    /// This increases the likelihood of introducing new topics in a response.
    /// </summary>
    [JsonPropertyName("presence_penalty")]
    public double PresencePenalty { get; set; } = 0;

    /// <summary>
    /// Set a limit on the number of tokens per model response. 
    /// The API supports a maximum of 2048 tokens shared between the prompt (including system message, examples,
    /// message history, and user query) and the model's response. One token is roughly 4 characters for typical English text.
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 60;

    /// <summary>
    /// Make responses stop at a desired point, such as the end of a sentence or list. 
    /// Specify up to four sequences where the model will stop generating further tokens in a response. 
    /// The returned text will not contain the stop sequence.
    /// </summary>
    [JsonPropertyName("stop")]
    public string? Stop { get; set; } = null;//"\n";
}
