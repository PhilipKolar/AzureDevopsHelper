using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevopsHelper.ResponseModels
{
    public class MemberCapacity
    {
        public string MemberId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public float CorrectCapacity { get; set; }
        public float CurrentCapacity { get; set; }
    }
}
