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

}

public class OpenAIDialogs : List<OpenAIDialog>
{
    private readonly string _tone;
    public OpenAIDialogs(string tone)
    {
        _tone = tone;
    }
    public void Add(DiscussionParticipant participant, string text)
    {
        this.Add(new OpenAIDialog { Participant = participant, Text = text });
    }

    //TODO: Should be in different assembly and inaccessible 
    internal string ToString(string chars = $"\n\n")
    {
        var conversation = string.Join(chars, this.Select(x => $"{x.Participant.ToString()}: {x.Text}"));

        return $"{_tone}{chars}{conversation}";
    }
}
