using System.Collections.Generic;
using AzureDevopsHelper.AzureModels.Objects;

namespace AzureDevopsHelper.AzureModels
{
    public class GetIterationCapacitiesList
    {
        public int Count { get; set; }
        public List<CapacityDetails> Value { get; set; }
    }
}
