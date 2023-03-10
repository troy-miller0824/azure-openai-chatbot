using Azure;
using AzureOpenAISample.Implementations;
using AzureOpenAISample.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AzureOpenAISample.Services;

public class DiscussionService : IDiscussionService
{
    private readonly ILogger<DiscussionService> _logger;
    private readonly IOpenAIService _openAIService;

    private readonly string CurrentTone = "You are Marv, a chatbot that reluctantly answers questions with sarcastic responses!";
    public OpenAIDialogs OpenAIDialogs { get; }

    public DiscussionService(ILoggerFactory loggerFactory, IOpenAIService openAIService)
    {
        _logger = loggerFactory.CreateLogger<DiscussionService>();
        _openAIService = openAIService;

        OpenAIDialogs = new OpenAIDialogs(CurrentTone);
    }


    public async Task<OpenAIDialog> GetResponseAsync(Guid discussionId, string question)
    {
        _logger.LogInformation($"Method {nameof(GetResponseAsync)} called successfully!");

        OpenAIDialogs.Add(discussionId, DiscussionParticipant.Human, question, DateTime.Now);

        var history = OpenAIDialogs.Where(x => x.DiscussionId == discussionId)
            .ToOpenAIDialogs(CurrentTone)
            .ToString();

        var response = await _openAIService.GetResponseAsync(history);
        var choice = response.Choices!.First();

        OpenAIDialogs.Add(discussionId, DiscussionParticipant.Marv, choice.Text!.Trim(), DateTime.Now);

        _logger.LogInformation($"Method {nameof(GetResponseAsync)} completed successfully!");

        return OpenAIDialogs
            .Where( x=> x.DiscussionId == discussionId)
            .OrderByDescending( x=> x.Timestamp)
            .First();
    }

    public OpenAIDialogs GetHistory(Guid discussionId)
    {
        return OpenAIDialogs.Where(x => x.DiscussionId == discussionId).ToOpenAIDialogs(CurrentTone);
    }
}
