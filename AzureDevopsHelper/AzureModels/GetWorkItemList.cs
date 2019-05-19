using System;
using System.Collections.Generic;
using System.Text;
using AzureDevopsHelper.AzureModels.Objects;

namespace AzureDevopsHelper.AzureModels
{
    public class GetWorkItemList
    {
        public int Count { get; set; }
        public List<WorkItemListWorkItem> Value { get; set; }
    }
}
