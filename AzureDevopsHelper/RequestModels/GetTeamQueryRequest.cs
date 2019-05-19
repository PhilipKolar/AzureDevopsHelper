using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper.RequestModels
{
    public class GetTeamQueryRequest
    {
        public string Name { get; set; }
        public string ProjectId { get; set; }
    }
}
