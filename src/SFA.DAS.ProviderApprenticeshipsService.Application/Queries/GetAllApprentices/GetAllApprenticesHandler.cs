using System;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAllApprentices
{
    public sealed class GetAllApprenticesHandler : IAsyncRequestHandler<GetAllApprenticesRequest, GetAllApprenticesResponse>
    {
        private ICommitmentsApi _commitmentsApi;

        public GetAllApprenticesHandler(ICommitmentsApi commitmentsApi)
        {
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));

            _commitmentsApi = commitmentsApi;
        }
                                                                                
        public async Task<GetAllApprenticesResponse> Handle(GetAllApprenticesRequest message)
        {
            var apprenticeship = await _commitmentsApi.GetProviderApprenticeships(message.ProviderId);

            return new GetAllApprenticesResponse
            {
                Apprenticeships = apprenticeship
                                    .Where(m => m.PaymentStatus != PaymentStatus.PendingApproval)
                                    .ToList()
            };            
        }
    }
}