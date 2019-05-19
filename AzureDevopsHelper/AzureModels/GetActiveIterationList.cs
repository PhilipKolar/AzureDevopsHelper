using System;
using System.Collections.Generic;
using System.Text;
using AzureDevopsHelper.AzureModels.Objects;

namespace AzureDevopsHelper.AzureModels
{
    public class GetActiveIterationList
    {
        public int Count { get; set; }
        public IEnumerable<Iteration> Value { get; set; }
    }
}
