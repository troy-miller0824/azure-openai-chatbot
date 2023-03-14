namespace AzureOpenAISample.Marv.Models;

public enum DiscussionParticipant { Human, Marv };

public class MarvDialog
{
    public DiscussionParticipant Participant { get; set; }
    public string? HumanText { get; set; }
    public string? BotText { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid DiscussionId { get; set; }
}

public static class OpenAiDialogsExtenstions
{
    public static string ToString(this IEnumerable<MarvDialog> marvDialogs, string template)
    {
        //Concat the text for Marv with the format:
        // MarvTone \n\n Human:... \n Marv:... \n Human:...
        var conversation = string.Join('\n', marvDialogs.Select(x => $"{x.Participant}: {(string.IsNullOrWhiteSpace(x.BotText) ? x.HumanText!.Trim().Replace('\n',',') : x.BotText!.Trim().Replace('\n', ','))}"));

        return string.Format(template, conversation);
    }

    public static void Add(this List<MarvDialog> marvDialogs, Guid discussionId, DiscussionParticipant participant, string humanText, string botText, DateTime timestamp)
    {
        marvDialogs.Add(
            new MarvDialog
            {
                DiscussionId = discussionId,
                Participant = participant,
                HumanText = humanText.Trim(),
                BotText = botText.Trim(),
                Timestamp = timestamp
            });
    }
}

