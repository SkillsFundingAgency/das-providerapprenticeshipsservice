using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipProvider
{
    public class ProvidersView
    {
        public DateTime CreatedDate { get; set; }
        public Provider Provider { get; set; }
    }
}