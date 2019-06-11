using System;
using System.Collections.Generic;
using System.Text;
using AzureDevopsHelper.AzureModels.Objects;

namespace AzureDevopsHelper.AzureModels
{
    public class GetIterationTeamDaysOff
    {
        public List<CapacityDateRange> DaysOff { get; set; }
    }
}
