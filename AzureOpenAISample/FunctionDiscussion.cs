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

    [Function(nameof(Response))]
    public async Task<HttpResponseData> Response([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        _logger.LogInformation($"Method {nameof(Response)} called successfully!");

        var content = await new StreamReader(req.Body).ReadToEndAsync();
        var result = await _discussionService.GetResponseAsync(content);
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteString(result);

        _logger.LogInformation($"Method {nameof(Response)} done processing successfully!");

        return response;
    }

    //[Function(nameof(Discussion)]
    //public async Task<HttpResponseData> Discussion([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    //{
    //    _logger.LogInformation($"Method {nameof(Discussion)} called successfully!");

    //    var content = await new StreamReader(req.Body).ReadToEndAsync();
    //    var result = await _discussionService.GetResponseAsync(content);
    //    var response = req.CreateResponse(HttpStatusCode.OK);
    //    response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
    //    response.WriteString(result);

    //    _logger.LogInformation($"Method {nameof(Discussion)} done processing successfully!");


    //    return response;
    //}
}
