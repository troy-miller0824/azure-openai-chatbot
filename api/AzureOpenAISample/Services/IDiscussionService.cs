using AzureOpenAISample.Models;

namespace AzureOpenAISample.Services;

public interface IDiscussionService
{
    Task<MarvDialog> GetResponseAsync(Guid discussionId, string question);
    MarvDialogs GetHistory(Guid discussionId);
}