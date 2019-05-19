using System;
using System.Collections.Generic;
using System.Text;
using AzureDevopsHelper.AzureModels.Objects;

namespace AzureDevopsHelper.AzureModels
{
    public class GetTeamList
    {
        public IEnumerable<Team> Value { get; set; }
    }
}
