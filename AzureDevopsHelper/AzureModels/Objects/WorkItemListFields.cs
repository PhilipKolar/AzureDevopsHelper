using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper.AzureModels.Objects
{
    public class WorkItemListFields
    {
        [JsonProperty("System.AreaPath")]
        public string AreaPath { get; set; }
        [JsonProperty("System.TeamProject")]
        public string TeamProject { get; set; }
        [JsonProperty("System.IterationPath")]
        public string IterationPath { get; set; }
        [JsonProperty("System.WorkItemType")]
        public string WorkItemType { get; set; }
        [JsonProperty("System.State")]
        public string State { get; set; }
        [JsonProperty("System.AssignedTo")]
        public WorkItemAssignedTo AssignedTo { get; set; }
        [JsonProperty("System.Title")]
        public string Title { get; set; }
        [JsonProperty("Microsoft.VSTS.Scheduling.RemainingWork")]
        public double RemainingWork { get; set; }
        [JsonProperty("Microsoft.VSTS.Common.Activity")]
        public string Activity { get; set; }
        [JsonProperty("Microsoft.VSTS.Scheduling.CompletedWork")]
        public double? CompletedWork { get; set; }

    }
}
