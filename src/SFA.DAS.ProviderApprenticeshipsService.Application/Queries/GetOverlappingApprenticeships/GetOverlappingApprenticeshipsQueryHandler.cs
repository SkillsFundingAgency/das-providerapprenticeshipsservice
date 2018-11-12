using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships
{
    public class GetOverlappingApprenticeshipsQueryHandler :IRequestHandler<GetOverlappingApprenticeshipsQueryRequest, GetOverlappingApprenticeshipsQueryResponse>
    {
        private readonly IValidationApi _validationApi;

        public GetOverlappingApprenticeshipsQueryHandler(IValidationApi validationApi)
        {
            _validationApi = validationApi;
        }

        public async Task<GetOverlappingApprenticeshipsQueryResponse> Handle(GetOverlappingApprenticeshipsQueryRequest request, CancellationToken cancellationToken)
        {
            var apprenticeships = request.Apprenticeship
                .Where(m =>
                    m.StartDate != null &&
                    m.EndDate != null &&
                    !string.IsNullOrEmpty(m.ULN))
                .ToList();

            if (!apprenticeships.Any())
            {
                return new GetOverlappingApprenticeshipsQueryResponse
                           {
                               Overlaps = Enumerable.Empty<ApprenticeshipOverlapValidationResult>()
                           };
            }

            return new GetOverlappingApprenticeshipsQueryResponse
            {
                Overlaps = await _validationApi
                .ValidateOverlapping(apprenticeships.Select(arg => new ApprenticeshipOverlapValidationRequest
                {
                    ApprenticeshipId = arg.Id,
                    Uln = arg.ULN,
                    StartDate = arg.StartDate.Value,
                    EndDate = arg.EndDate.Value
                }))
            };
        }
    }
}