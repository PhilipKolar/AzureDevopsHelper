using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper.AzureModels.Objects
{
    public class TeamMember
    {
        public TeamMemberIdentity identity { get; set; }
        public bool? IsTeamAdmin { get; set; }
    }
}
