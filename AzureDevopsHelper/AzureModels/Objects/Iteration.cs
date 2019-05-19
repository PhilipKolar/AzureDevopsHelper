using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper.AzureModels.Objects
{
    public class Iteration
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public IterationAttributes Attributes { get; set; }
        public string Url { get; set; }
    }
}
