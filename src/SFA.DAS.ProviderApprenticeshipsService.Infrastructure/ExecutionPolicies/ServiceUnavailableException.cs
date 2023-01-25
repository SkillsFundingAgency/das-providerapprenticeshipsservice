using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.ExecutionPolicies
{
    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException()
            : base("Service is unavailable")
        {
        }
    }
}