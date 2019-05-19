using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper.RequestModels
{
    public class GetTeamMembersQueryRequest
    {
        public string ProjectId { get; set; }
        public string TeamId { get; set; }
    }
}
