using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper.RequestModels
{
    public class GetCorrectCapacityQueryRequest
    {
        public string IterationId { get; set; }
        public DateTime IterationStartDate { get; set; }
        public DateTime IterationEndDate { get; set; }
    }
}
