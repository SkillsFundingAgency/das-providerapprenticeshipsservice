using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider
{
    public class GetProviderQueryHandler : IRequestHandler<GetProviderQueryRequest, GetProviderQueryResponse>
    {
        private readonly IApprenticeshipInfoService _apprenticeshipInfoService;

        public GetProviderQueryHandler(IApprenticeshipInfoService apprenticeshipInfoService)
        {
            _apprenticeshipInfoService = apprenticeshipInfoService;
        }

        public async Task<GetProviderQueryResponse> Handle(GetProviderQueryRequest message, CancellationToken cancellationToken)
        {
            var provider = _apprenticeshipInfoService.GetProvider(message.UKPRN);

            return await Task.FromResult(new GetProviderQueryResponse
            {
                ProvidersView = provider
            });
        }
    }
}