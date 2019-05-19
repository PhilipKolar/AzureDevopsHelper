using System;
using System.Collections.Generic;
using System.Text;
using AzureDevopsHelper.ResponseModels;

namespace AzureDevopsHelper.RequestModels
{
    public class GetCurrentCapacityQueryRequest
    {
        public IEnumerable<MemberCapacity> CorrectMemberCapacities { get; set; }
        public string IterationPath { get; set; }
    }
}
