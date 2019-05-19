using System;
using System.Collections.Generic;
using System.Text;
using AzureDevopsHelper.AzureModels.Objects;

namespace AzureDevopsHelper.AzureModels
{
    public class PostWiqlQuery
    {
        public string QueryType { get; set; }
        public string QueryResultType { get; set; }
        public DateTime AsOf { get; set; }
        public List<WiqlColumn> Columns { get; set; }
        public List<WiqlSortColumn> SortColumns { get; set; }
        public List<WiqlWorkItem> WorkItems { get; set; }
    }
}
