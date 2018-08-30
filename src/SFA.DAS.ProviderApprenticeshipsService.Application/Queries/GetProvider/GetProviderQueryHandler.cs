using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider
{
    public class GetProviderQueryHandler : IAsyncRequestHandler<GetProviderQueryRequest, GetProviderQueryResponse>
    {
        private readonly IApprenticeshipInfoService _apprenticeshipInfoService;

        public GetProviderQueryHandler(IApprenticeshipInfoService apprenticeshipInfoService)
        {
            if (apprenticeshipInfoService == null)
                throw new ArgumentNullException(nameof(apprenticeshipInfoService));
            _apprenticeshipInfoService = apprenticeshipInfoService;
        }

        public async Task<GetProviderQueryResponse> Handle(GetProviderQueryRequest message)
        {
            var provider = _apprenticeshipInfoService.GetProvider(message.UKPRN);

            return await Task.FromResult(new GetProviderQueryResponse
            {
                ProvidersView = provider
            });
        }
    }
}