namespace AzureOpenAISample.Services;

public interface IDiscussionService
{
    Task<string> GetResponseAsync(string question);
}