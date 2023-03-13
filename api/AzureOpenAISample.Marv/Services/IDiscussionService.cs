using AzureOpenAISample.Marv.Models;

namespace AzureOpenAISample.Marv.Services;

public interface IDiscussionService
{
    Task<MarvDialog> GetResponseAsync(Guid discussionId, string question);

    List<MarvDialog> GetHistory(Guid discussionId);
}