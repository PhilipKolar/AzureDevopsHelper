using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper.AzureModels.Objects
{
    public class TeamMemberIdentity
    {
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public Links Links { get; set; }
        public string Id { get; set; }
        public string UniqueName { get; set; }
        public string ImageUrl { get; set; }
        public bool IsContainer { get; set; }
        public string Descriptor { get; set; }
    }
}
