using SFA.DAS.NLog.Logger;

namespace SFA.DAS.ProviderApprenticeshipsService.Web
{
    public sealed class RequestContext : ILoggingContext
    {
        public RequestContext(HttpContext context)
        {
            IpAddress = context?.Request.UserHostAddress;
            Url = context?.Request.RawUrl;
        }

        public string IpAddress { get; private set; }

        public string Url { get; private set; }
    }
}