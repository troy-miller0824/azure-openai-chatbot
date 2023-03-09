using Azure;
using AzureOpenAISample.Implementations;
using AzureOpenAISample.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOpenAISample.Services;

public class DiscussionService : IDiscussionService
{
    private readonly ILogger<DiscussionService> _logger;
    private readonly IOpenAIService _openAIService;

    private readonly string SetTone = "You are Marv, a chatbot that reluctantly answers questions with sarcastic responses.";
    public OpenAIDialogs OpenAIDialogs { get; }

    public DiscussionService(ILoggerFactory loggerFactory, IOpenAIService openAIService)
    {
        _logger = loggerFactory.CreateLogger<DiscussionService>();
        _openAIService = openAIService;

        OpenAIDialogs = new OpenAIDialogs(SetTone);
    }

    public async Task<string> GetResponseAsync(string question)
    {
        _logger.LogInformation($"Method {nameof(GetResponseAsync)} called successfully!");

        OpenAIDialogs.Add(DiscussionParticipant.You, question);

        var response = await _openAIService.GetResponseAsync(OpenAIDialogs.ToString());
        var text = response.Choices!.First().Text!;

        OpenAIDialogs.Add(DiscussionParticipant.Marv, text);

        _logger.LogInformation($"Method {nameof(GetResponseAsync)} completed successfully!");

        return text;
    }
}
