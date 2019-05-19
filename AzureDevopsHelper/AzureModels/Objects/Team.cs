using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper.AzureModels.Objects
{
    public class Team
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string IdentityUrl { get; set; }
        public string ProjectName { get; set; }
        public string ProjectId { get; set; }
    }
}
