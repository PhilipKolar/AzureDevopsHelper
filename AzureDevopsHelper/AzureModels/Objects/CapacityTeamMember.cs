using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper.AzureModels.Objects
{
    public class CapacityTeamMember
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string UniqueName { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
    }
}
