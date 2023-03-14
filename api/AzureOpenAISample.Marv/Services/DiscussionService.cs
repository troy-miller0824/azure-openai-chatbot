using AzureOpenAISample.Marv.Implementations;
using AzureOpenAISample.Marv.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureOpenAISample.Marv.Services;

public class DiscussionService : IDiscussionService
{
    private readonly ILogger<DiscussionService> _logger;
    private readonly IOpenAIService _openAIService;
    private readonly IConfiguration _configuration;

    private readonly string MarvTemplate = "Marv is a chatbot that reluctantly answers questions with sarcastic responses:\n\n{0}\nMarv:";
    private readonly string DirkTemplate = "Correct the following sentences in standard EnglishL\n\nSentence: \"how,\"\nCorrect: \"How?\"\n\nSentence: \"I am george,\"\nCorrect: \"I am George.\"\n\nSentence:\"hi\"\nCorrect: \"Hi!\"\n\nSentence: \"{0}\"\nCorrect:";

    public List<MarvDialog> OpenAIDialogs { get; } = new List<MarvDialog>();

    public DiscussionService(IConfiguration configuration, ILoggerFactory loggerFactory, IOpenAIService openAIService)
    {
        _logger = loggerFactory.CreateLogger<DiscussionService>();
        _openAIService = openAIService;
        _configuration = configuration;
    }


    public async Task<MarvDialog> GetResponseAsync(Guid discussionId, string question)
    {
        _logger.LogInformation($"Discussion {discussionId}, Question: {question}!");

        //have dirk correct the question and then add both version to OpenAIDialogs collection
        var dirkQuestion = await DirkCorrectionAsync(discussionId, question);
        OpenAIDialogs.Add(discussionId, DiscussionParticipant.Human, question, dirkQuestion, DateTime.Now);

        //Get the history for this discussionid 
        var history = OpenAIDialogs.Where(x => x.DiscussionId == discussionId).ToString(MarvTemplate);

        //ask Marv add the response, and return what he said
        var marvModelName = _configuration.GetValue<string>(OpenAISettingNames.MarvModelName) ?? throw new NullReferenceException($"{OpenAISettingNames.MarvModelName} is null or empty!");
        var response = await _openAIService.GetResponseAsync(history, marvModelName);

        if (response.Choices != null && response.Choices.Any())
        {
            var choice = response.Choices.First();
            if (choice.FinishReason == "content_filter")
            {
                _logger.LogWarning($"Marv failed to answer question '{question}' for discussion '{discussionId}'.");
                return new MarvDialog
                {
                    BotText = "What you 've asked triggered my content filters and I cannot continue! Try rephrasing it!",
                    DiscussionId = discussionId,
                    HumanText = string.Empty,
                    Participant = DiscussionParticipant.Marv,
                    Timestamp = DateTime.Now
                };
            }
            else
            {
                var botText = choice.Text!.Trim().Trim('"').Trim();
                if (botText.StartsWith(DiscussionParticipant.Marv.ToString()))
                {
                    botText = botText.Remove(0, DiscussionParticipant.Marv.ToString().Length).TrimStart(':').Trim();
                }
                OpenAIDialogs.Add(discussionId, DiscussionParticipant.Marv, string.Empty, botText, DateTime.Now);
                _logger.LogInformation($"Marv answered question '{question}' for discussion '{discussionId}' : {choice.Text}!");

            }
        }
        else
        {
            _logger.LogWarning($"Marv failed to return any choices for question '{question}' and discussion '{discussionId}'.");
            return new MarvDialog
            {
                BotText = "What you 've asked triggered my content filtering and I cannot continue! Try rephrasing it!",
                DiscussionId = discussionId,
                HumanText = string.Empty,
                Participant = DiscussionParticipant.Marv,
                Timestamp = DateTime.Now
            };
        }

        return OpenAIDialogs
            .Where(x => x.DiscussionId == discussionId)
            .OrderByDescending(x => x.Timestamp)
            .First();
    }

    private async Task<string> DirkCorrectionAsync(Guid discussionId, string question)
    {
        var textReturn = string.Empty;

        if (!_configuration.GetValue<bool>(OpenAISettingNames.UseDirk))
        {
            //remove comma and its destructive power!
            return question.Trim().TrimEnd(',').Replace("\"", "\\u0022");
        }

        var dirkModelName = _configuration.GetValue<string>(OpenAISettingNames.DirkModelName) ?? throw new NullReferenceException($"{OpenAISettingNames.DirkModelName} is null or empty!"); ;

        //using comma at the end is my nemesis because it is instructing the bot to continue the same phrase.
        //addiotionally, if user included double quotes it will confuse dirk since we are adding double quoutes.
        question = question.TrimEnd(',').Replace("\"", "\\u0022");
        var response = await _openAIService.GetResponseAsync(string.Format(DirkTemplate, question), dirkModelName);

        //get the response but remove double quotes, dirk answers the way it was asked
        if (response.Choices != null && response.Choices.Any())
        {
            var choice = response.Choices.First();
            if (choice.FinishReason == "content_filter")
            {
                textReturn = string.Empty;//means content filtered
                _logger.LogWarning($"Dirk failed to correct question '{question}' for discussion '{discussionId}'.");
            }
            else
            {
                textReturn = choice.Text!.Trim().Trim('"').Trim().ToLower();
                _logger.LogInformation($"Dirk corrected question '{question}' for discussion '{discussionId}' : {textReturn}!");
            }
        }
        else
        {
            //if no choices returneed, most probably the response was contented filtered as whole
            textReturn = question;
            _logger.LogWarning($"Dirk failed to return any {nameof(response.Choices)} for question '{question}' and discussion '{discussionId}'.");
        }

        return textReturn;
    }

    public List<MarvDialog> GetHistory(Guid discussionId)
    {
        return OpenAIDialogs.Where(x => x.DiscussionId == discussionId).OrderBy(x => x.Timestamp).ToList();
    }
}
