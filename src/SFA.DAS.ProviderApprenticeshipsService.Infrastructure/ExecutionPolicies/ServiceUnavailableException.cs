using System.Web;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.ExecutionPolicies
{
    public class ServiceUnavailableException : HttpException
    {
        public ServiceUnavailableException()
            : base(503, "Service is unavailable")
        {
        }
    }
}