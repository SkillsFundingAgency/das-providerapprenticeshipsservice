using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship
{
    public class GetApprenticeshipQueryHandler : IAsyncRequestHandler<GetApprenticeshipQueryRequest, GetApprenticeshipQueryResponse>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;

        public GetApprenticeshipQueryHandler(IProviderCommitmentsApi providerCommitmentsApi)
        {
            if (providerCommitmentsApi == null)
                throw new ArgumentNullException(nameof(providerCommitmentsApi));
            _commitmentsApi = providerCommitmentsApi;
        }

        public async Task<GetApprenticeshipQueryResponse> Handle(GetApprenticeshipQueryRequest message)
        {
            var apprenticeship = await _commitmentsApi.GetProviderApprenticeship(message.ProviderId, message.ApprenticeshipId);

            return new GetApprenticeshipQueryResponse
            {
                Apprenticeship = apprenticeship
            };
        }
    }
}