using AzureOpenAISample.Marv.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOpenAISample.Models;

public class Dialog
{
    public DiscussionParticipant Participant { get; set; }
    public string? HumanText { get; set; }
    public string? BotText { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid DiscussionId { get; set; }
}
