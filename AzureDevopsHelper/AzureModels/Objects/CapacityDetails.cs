using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper.AzureModels.Objects
{
    public class CapacityDetails
    {
        public CapacityTeamMember TeamMember { get; set; }
        public List<Activity> Activities { get; set; }
        public List<CapacityDateRange> DaysOff { get; set; }
        public string Url { get; set; }
    }
}
