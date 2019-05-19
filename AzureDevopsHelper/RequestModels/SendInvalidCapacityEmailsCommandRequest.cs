using System.Collections.Generic;
using AzureDevopsHelper.ResponseModels;

namespace AzureDevopsHelper.RequestModels
{
    public class SendInvalidCapacityEmailsCommandRequest
    {
        public List<MemberCapacity> InvalidCapacities { get; set; }
    }
}
