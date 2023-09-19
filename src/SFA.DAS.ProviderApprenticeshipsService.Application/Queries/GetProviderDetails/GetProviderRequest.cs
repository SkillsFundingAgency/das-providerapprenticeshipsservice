using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderDetails
{
    public class GetProviderRequest : IGetApiRequest
    {
        private readonly long _ukprn;

        public GetProviderRequest(long ukprn)
        {
            _ukprn = ukprn;
        }

        public string GetUrl => $"providers/{_ukprn}";
    }
}
