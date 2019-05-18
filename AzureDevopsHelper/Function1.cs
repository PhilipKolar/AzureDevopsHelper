using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureDevopsHelper
{
    public static class Function1
    {
        [FunctionName("TimeCompletedTracker")]
        public static void Run([TimerTrigger("0 */5 * * * *"
#if DEBUG
            , RunOnStartup = true
#endif
            )]TimerInfo timer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
