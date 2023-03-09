using AzureOpenAISample.Implementations;
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

    [Function("Function1")]
    public async Task<HttpResponseData> Function1([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation($"{nameof(Function1)} called successfully!");

        var result = await _discussionService.ResponseAsync("What is HTML?");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString(result);

        return response;
    }
}
