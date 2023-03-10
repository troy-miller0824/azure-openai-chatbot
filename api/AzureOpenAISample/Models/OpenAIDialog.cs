using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOpenAISample.Models;

public enum DiscussionParticipant { Human, Marv };
public class OpenAIDialog
{
    public DiscussionParticipant Participant { get; set; }
    public string? Text { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid DiscussionId { get; set; }
}

public class OpenAIDialogs : List<OpenAIDialog>
{
    private readonly string _tone;
    public OpenAIDialogs(string tone)
    {
        _tone = tone;
    }



    public void Add(Guid discussionId, DiscussionParticipant participant, string text, DateTime timestamp)
    {
        this.Add(
            new OpenAIDialog
            {
                DiscussionId = discussionId,
                Participant = participant,
                Text = text.Trim().TrimStart(participant.ToString().ToCharArray()).TrimStart(':').Trim(),
                Timestamp = timestamp
            });
    }

    //TODO: Should be in different assembly and inaccessible 
    internal string ToString(string chars = $"\n\n")
    {
        var conversation = string.Join(chars, this.Select(x => $"{x.Participant.ToString()}: {x.Text!.Trim()}"));//{x.Participant.ToString()}: 

        return $"{_tone}{chars}{conversation}";
    }
}

public static class OpenAiDialogsExtenstions
{
    public static OpenAIDialogs ToOpenAIDialogs(this IEnumerable<OpenAIDialog> dialogs, string tone)
    {
        var openAiDialogs = new OpenAIDialogs(tone);
        openAiDialogs.AddRange(dialogs);
        return openAiDialogs;
    }
}

