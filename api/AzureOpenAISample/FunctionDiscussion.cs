using AzureOpenAISample.Marv.Services;
using AzureOpenAISample.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace AzureOpenAISample;

public class FunctionDiscussion
{
    private readonly ILogger _logger;
    private readonly IDiscussionService _discussionService;

    public FunctionDiscussion(ILoggerFactory loggerFactory, IDiscussionService discussionService)
    {
        _discussionService = discussionService;
        _logger = loggerFactory.CreateLogger<FunctionDiscussion>();
    }

    [Function($"{nameof(AskMarv)}/{{discussionId:guid}}")]
    public async Task<MultiResponse> AskMarv([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req, Guid discussionId)
    {
        _logger.LogInformation($"Method {nameof(AskMarv)} called successfully!");

        var content = await new StreamReader(req.Body).ReadToEndAsync();
        var result = await _discussionService.GetResponseAsync(discussionId, content);
        var response = req.CreateResponse(HttpStatusCode.Accepted);
        await response.WriteAsJsonAsync(result);

        var history = JsonSerializer.Serialize(
                                _discussionService.GetHistory(discussionId),
                                new JsonSerializerOptions { WriteIndented = true });

        return new MultiResponse()
        {
            // Write a single message.
            BlobOutput = history,
            HttpResponseData = response
        };

    }

    [Function($"{nameof(History)}/{{discussionId:guid}}")]
    public async Task<HttpResponseData> History(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
        [BlobInput("discussions/{discussionId}.json")] string discussion,
        Guid discussionId)
    {
        _logger.LogInformation($"Method {nameof(History)} called successfully!");

        var json = JsonSerializer.Deserialize<List<Dialog>>(discussion);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(json);

        return response;
    }
}
