using System;
using System.Collections.Generic;
using System.Text;
using AzureDevopsHelper.AzureModels.Objects;

namespace AzureDevopsHelper.AzureModels
{
    public class GetProjectList
    {
        public int Count { get; set; }
        public IEnumerable<Project> Value { get; set; }
    }
}
