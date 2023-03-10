using AzureOpenAISample.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

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
    public async Task<HttpResponseData> AskMarv([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req, Guid discussionId)
    {
        _logger.LogInformation($"Method {nameof(AskMarv)} called successfully!");

        var content = await new StreamReader(req.Body).ReadToEndAsync();
        var result = await _discussionService.GetResponseAsync(discussionId, content);
        var response = req.CreateResponse(HttpStatusCode.Accepted);
        await response.WriteAsJsonAsync(result);

        _logger.LogInformation($"Method {nameof(AskMarv)} done processing successfully!");

        return response;
    }

    [Function($"{nameof(History)}/{{discussionId:guid}}")]
    public async Task<HttpResponseData> History([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, Guid discussionId)
    {
        _logger.LogInformation($"Method {nameof(History)} called successfully!");

        var result = _discussionService.GetHistory(discussionId);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result);

        _logger.LogInformation($"Method {nameof(History)} done processing successfully!");

        return response;
    }
}
