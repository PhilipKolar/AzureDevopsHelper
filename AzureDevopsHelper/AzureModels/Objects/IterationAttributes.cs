using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper.AzureModels.Objects
{
    public class IterationAttributes
    {
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string TimeFrame { get; set; }
    }
}
