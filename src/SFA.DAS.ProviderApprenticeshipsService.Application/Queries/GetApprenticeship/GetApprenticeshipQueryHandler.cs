using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship
{
    public class GetApprenticeshipQueryHandler : IRequestHandler<GetApprenticeshipQueryRequest, GetApprenticeshipQueryResponse>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;

        public GetApprenticeshipQueryHandler(IProviderCommitmentsApi providerCommitmentsApi)
        {
            _commitmentsApi = providerCommitmentsApi;
        }

        public async Task<GetApprenticeshipQueryResponse> Handle(GetApprenticeshipQueryRequest message, CancellationToken cancellationToken)
        {
            var apprenticeship = await _commitmentsApi.GetProviderApprenticeship(message.ProviderId, message.ApprenticeshipId);

            return new GetApprenticeshipQueryResponse
            {
                Apprenticeship = apprenticeship
            };
        }
    }
}