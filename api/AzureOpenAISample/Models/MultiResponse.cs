using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureOpenAISample.Models;
public class MultiResponse
{
    [BlobOutput("discussions/{discussionId}.json")]
    public string? BlobOutput { get; set; }
    public HttpResponseData? HttpResponseData { get; set; }
}
