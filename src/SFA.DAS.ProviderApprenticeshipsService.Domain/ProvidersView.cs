using System;
using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain
{
    public class ProvidersView
    {
        public DateTime CreatedDate { get; set; }
        public List<Provider> Providers { get; set; }
    }
}