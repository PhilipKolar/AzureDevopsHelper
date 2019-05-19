using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper
{
    public class ConfigContainer
    {
        public string PersonalAccessToken { get; set; }
        public string OrganisationName { get; set; }
        public string AzureDevopsApiVersion { get; set; }
        public string ProjectName { get; set; }
        public string TeamName { get; set; }
        public bool CountDaysOff { get; set; }
        public List<string> ExclusionList { get; set; }
        public string EmailCredentialsUserName { get; set; }
        public string EmailCredentialsPassword { get; set; }
        public string EmailHost { get; set; }
        public int EmailPort { get; set; }
        public bool EmailEnableSsl { get; set; }
    }
}
