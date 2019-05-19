using System;
using System.Collections.Generic;
using System.Text;
using AzureDevopsHelper.AzureModels.Objects;

namespace AzureDevopsHelper.ResponseModels
{
    public class GetTeamMembersQueryResponse
    {
        public IEnumerable<TeamMember> TeamMembers { get; set; }
    }
}
