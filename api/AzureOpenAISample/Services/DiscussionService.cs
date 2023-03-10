using Azure;
using AzureOpenAISample.Implementations;
using AzureOpenAISample.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AzureOpenAISample.Services;

public class DiscussionService : IDiscussionService
{
    private readonly ILogger<DiscussionService> _logger;
    private readonly IOpenAIService _openAIService;
    private readonly IConfiguration _configuration;

    private readonly string MarvTone = "You are Marv, a chatbot that reluctantly answers questions with sarcastic responses!";
    private readonly string DirkTone = "Correct the following, grammatically and syntactically:";

    public MarvDialogs OpenAIDialogs { get; }

    public DiscussionService(IConfiguration configuration, ILoggerFactory loggerFactory, IOpenAIService openAIService)
    {
        _logger = loggerFactory.CreateLogger<DiscussionService>();
        _openAIService = openAIService;
        _configuration = configuration;

        OpenAIDialogs = new MarvDialogs(MarvTone);
    }


    public async Task<MarvDialog> GetResponseAsync(Guid discussionId, string question)
    {
        _logger.LogInformation($"Discussion {discussionId}, Question: {question}!");

        //have dirk correct the question and then add both version to OpenAIDialogs collection
        var dirkQuestion = await DirkCorrectionAsync(discussionId, question);
        OpenAIDialogs.Add(discussionId, DiscussionParticipant.Human, question, dirkQuestion, DateTime.Now);

        //Get the latest history for this discussionid 
        var history = OpenAIDialogs.Where(x => x.DiscussionId == discussionId)
            .ToOpenAIDialogs(MarvTone)
            .ToString();

        //ask Marv add the response, and return what he said
        var marvModelName = _configuration.GetValue<string>(OpenAISettingNames.MarvModelName);
        var response = await _openAIService.GetResponseAsync(history, marvModelName);
        var botText = response.Choices!.First().Text!.Trim();

        OpenAIDialogs.Add(discussionId, DiscussionParticipant.Marv, string.Empty, botText, DateTime.Now);
        _logger.LogInformation($"Marv was asked for discussion '{discussionId}' and question '{question}': {botText}!");

        return OpenAIDialogs
            .Where( x=> x.DiscussionId == discussionId)
            .OrderByDescending( x=> x.Timestamp)
            .First();
    }

    private async Task<string> DirkCorrectionAsync(Guid discussionId, string question)
    {
        var dirkModelName = _configuration.GetValue<string>(OpenAISettingNames.MarvModelName);
        //if user had double quotes it will confuse dirk since we are adding double quoutes: DirkTone: "question
        question = question.Replace('"', '\'');
        var response = await _openAIService.GetResponseAsync($"{DirkTone}\"{question}\"", dirkModelName);
        //get the response but remove double quotes, dirk answers the way it was asked
        var text = response.Choices!.First().Text!.Trim().Trim('"');

        _logger.LogInformation($"Dirk corrected for discussion '{discussionId}' and question '{question}': {text}!");

        return text;
    }

    public MarvDialogs GetHistory(Guid discussionId)
    {
        return OpenAIDialogs.Where(x => x.DiscussionId == discussionId).ToOpenAIDialogs(MarvTone);
    }
}
