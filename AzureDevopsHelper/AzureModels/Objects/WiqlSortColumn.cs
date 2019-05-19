using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper.AzureModels.Objects
{
    public class WiqlSortColumn
    {
        public WiqlField Field { get; set; }
        public bool Descending { get; set; }
    }
}
