using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper.ResponseModels
{
    public class GetCorrectCapacityQueryResponse
    {
        public IEnumerable<MemberCapacity> MemberCapacities { get; set; }
    }
}
