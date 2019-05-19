using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper.AzureModels.Objects
{
    public class WorkItemListWorkItem
    {
        public int Id { get; set; }
        public int Rev { get; set; }
        public WorkItemListFields Fields { get; set; }
        public string Url { get; set; }
    }
}
