using AzureOpenAISample.Models;

namespace AzureOpenAISample.Services;

public interface IDiscussionService
{
    Task<OpenAIDialog> GetResponseAsync(Guid discussionId, string question);
    OpenAIDialogs GetHistory(Guid discussionId);
}