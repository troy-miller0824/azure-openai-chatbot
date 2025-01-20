using System.Text.Json.Serialization;

namespace AzureOpenAISample.Marv.Models;
public class Choice
{
    public ContentFilterResults content_filter_results { get; set; } = new ContentFilterResults();
    public string? finish_reason { get; set; }
    public int index { get; set; }
    public object? logprobs { get; set; }
    public Message message { get; set; } = new Message();
}

public class CompletionTokensDetails
{
    public int accepted_prediction_tokens { get; set; }
    public int audio_tokens { get; set; }
    public int reasoning_tokens { get; set; }
    public int rejected_prediction_tokens { get; set; }
}

public class ContentFilterResults
{
    public Hate hate { get; set; } = new Hate();
    public SelfHarm self_harm { get; set; } = new SelfHarm();
    public Sexual sexual { get; set; } = new Sexual();
    public Violence violence { get; set; } = new Violence();
    public Jailbreak jailbreak { get; set; } = new Jailbreak();
}

public class Hate
{
    public bool filtered { get; set; }
    public string? severity { get; set; }
}

public class Jailbreak
{
    public bool filtered { get; set; }
    public bool detected { get; set; }
}

public class Message
{
    public string? content { get; set; }
    public object? refusal { get; set; }
    public string? role { get; set; }
}

public class PromptFilterResult
{
    public int prompt_index { get; set; }
    public ContentFilterResults content_filter_results { get; set; } = new ContentFilterResults();
}

public class PromptTokensDetails
{
    public int audio_tokens { get; set; }
    public int cached_tokens { get; set; }
}

public class OpenAIResponse
{
    public List<Choice> choices { get; set; } = new List<Choice>();
    public int created { get; set; }
    public string? id { get; set; }
    public string? model { get; set; }
    public string? @object { get; set; }
    public List<PromptFilterResult> prompt_filter_results { get; set; } = new List<PromptFilterResult>();
    public string? system_fingerprint { get; set; }
    public Usage usage { get; set; } = new Usage();
}

public class SelfHarm
{
    public bool filtered { get; set; }
    public string? severity { get; set; }
}

public class Sexual
{
    public bool filtered { get; set; }
    public string? severity { get; set; }
}

public class Usage
{
    public int completion_tokens { get; set; }
    public CompletionTokensDetails completion_tokens_details { get; set; } = new CompletionTokensDetails();
    public int prompt_tokens { get; set; }
    public PromptTokensDetails prompt_tokens_details { get; set; } = new PromptTokensDetails();
    public int total_tokens { get; set; }
}

public class Violence
{
    public bool filtered { get; set; }
    public string? severity { get; set; }
}

