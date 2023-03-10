namespace AzureOpenAISample.Models;

public enum DiscussionParticipant { Human, Marv };
public class MarvDialog
{
    public DiscussionParticipant Participant { get; set; }
    public string? HumanText { get; set; }
    public string? BotText { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid DiscussionId { get; set; }
}

public class MarvDialogs : List<MarvDialog>
{
    private readonly string _tone;
    public MarvDialogs(string tone)
    {
        _tone = tone;
    }

    public void Add(Guid discussionId, DiscussionParticipant participant, string humanText, string botText, DateTime timestamp)
    {
        //Marv is asked in the format Human:... \n Marv:... \n Human:...
        // meaning with the participant name as prefix before every question,
        // so the answer he gives back is in the format "Marv: ...."
        //We remove the "Marv:" here.
        if (botText.Trim().StartsWith(participant.ToString()))
        {
            botText = botText.Trim().Remove(0, participant.ToString().Length).TrimStart(':');
        }

        this.Add(
            new MarvDialog
            {
                DiscussionId = discussionId,
                Participant = participant,
                HumanText = humanText,
                BotText = botText.Trim(),
                Timestamp = timestamp
            });
    }

    public override string ToString()
    {
        //Concat the text for Marv with the format:
        // MarvTone \n\n Human:... \n Marv:... \n Human:...
        var conversation = string.Join('\n', this.Select(x => $"{x.Participant}: {x.BotText!.Trim()}"));//{x.Participant.ToString()}: 

        return $"{_tone}\n\n{conversation}";
    }
}

public static class OpenAiDialogsExtenstions
{
    public static MarvDialogs ToOpenAIDialogs(this IEnumerable<MarvDialog> dialogs, string tone)
    {
        var openAiDialogs = new MarvDialogs(tone);
        openAiDialogs.AddRange(dialogs);
        return openAiDialogs;
    }
}

