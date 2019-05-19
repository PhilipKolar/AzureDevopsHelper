using System;
using System.Collections.Generic;
using System.Text;
using AzureDevopsHelper.AzureModels.Objects;

namespace AzureDevopsHelper.AzureModels
{
    public class GetTeamMembersList
    {
        public int Count { get; set; }
        public IEnumerable<TeamMember> Value { get; set; }
    }
}
