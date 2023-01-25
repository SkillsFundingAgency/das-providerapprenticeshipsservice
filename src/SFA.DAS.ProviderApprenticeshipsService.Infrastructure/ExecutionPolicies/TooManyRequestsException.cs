using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.ExecutionPolicies
{
    public class TooManyRequestsException : Exception
    {
        public TooManyRequestsException()
            : base("Rate limit has been reached")
        {
        }
    }
}