using System.Web;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.ExecutionPolicies
{
    public class TooManyRequestsException : HttpException
    {
        public TooManyRequestsException()
            : base(429, "Rate limit has been reached")
        {
        }
    }
}