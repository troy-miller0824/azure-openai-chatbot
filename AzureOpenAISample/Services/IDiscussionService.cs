namespace AzureOpenAISample.Services;

public interface IDiscussionService
{
    Task<string> ResponseAsync(string question);
}