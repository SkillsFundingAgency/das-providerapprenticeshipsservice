using System;
using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain
{
    public class StandardsView
    {
        public DateTime CreationDate { get; set; }
        public List<Standard> Standards { get; set; }
    }
}