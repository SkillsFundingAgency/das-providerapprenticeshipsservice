namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Http
{
    public class TooManyRequestsException : HttpException
    {
        public TooManyRequestsException()
            : base(429, "Rate limit has been reached")
        {
        }
    }
}
