using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships
{
    public class GetOverlappingApprenticeshipsQueryHandler :
        IAsyncRequestHandler<GetOverlappingApprenticeshipsQueryRequest, GetOverlappingApprenticeshipsQueryResponse>
    {
        private readonly IProviderCommitmentsApi _providerCommitmentsApi;

        public GetOverlappingApprenticeshipsQueryHandler(IProviderCommitmentsApi commitmentsApi)
        {
            if(commitmentsApi == null)
                throw new ArgumentException(nameof(commitmentsApi));
            _providerCommitmentsApi = commitmentsApi;
        }

        public async Task<GetOverlappingApprenticeshipsQueryResponse> Handle(GetOverlappingApprenticeshipsQueryRequest request)
        {
            var apprenticeships = request.Apprenticeship.Where(m =>
                                                               m.StartDate != null &&
                                                               m.EndDate != null &&
                                                               !string.IsNullOrEmpty(m.ULN));
            
            if (!apprenticeships.Any())
            {
                return new GetOverlappingApprenticeshipsQueryResponse
                           {
                               Overlaps =
                                   Enumerable.Empty<OverlapApprenticeship>()
                           };
            }

            // ToDo: Make call if any valid
            //var result = _commitmentsApi.Validation.ValidateApprenticeship(request.Apprenticeship);
            var model = new GetOverlappingApprenticeshipsQueryResponse
            {
                Overlaps = new List<OverlapApprenticeship>
                               {
                                   new OverlapApprenticeship
                                       {
                                           EmployerAccountId = 123456,
                                           LegalEntityName = "Legal entity name",
                                           ProviderId = 665544,
                                           ProviderName = "Provider name!",
                                           ValidationFailReason = "StartDate"
                                       },
                                   new OverlapApprenticeship
                                       {
                                           EmployerAccountId = 123456,
                                           LegalEntityName = "Legal entity name 2",
                                           ProviderId = 665544,
                                           ProviderName = "Provider name! 2",
                                           ValidationFailReason = "EndDate"
                                       }
                               }
            };
            return model;
        }
    }
}