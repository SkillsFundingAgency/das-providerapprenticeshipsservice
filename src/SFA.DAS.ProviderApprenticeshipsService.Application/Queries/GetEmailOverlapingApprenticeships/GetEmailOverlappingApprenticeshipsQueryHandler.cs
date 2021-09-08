using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Validation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetEmailOverlapingApprenticeships
{
    public class GetEmailOverlappingApprenticeshipsQueryHandler : IRequestHandler<GetEmailOverlappingApprenticeshipsQueryRequest, GetEmailOverlappingApprenticeshipsQueryResponse>
    {
        private readonly IValidationApi _validationApi;

        public GetEmailOverlappingApprenticeshipsQueryHandler(IValidationApi validationApi)
        {
            _validationApi = validationApi;
        }

        public async Task<GetEmailOverlappingApprenticeshipsQueryResponse> Handle(GetEmailOverlappingApprenticeshipsQueryRequest request, CancellationToken cancellationToken)
        {
            var apprenticeships = request.Apprenticeship
                .Where(m =>
                    m.StartDate != null &&
                    m.EndDate != null &&
                    !string.IsNullOrEmpty(m.Email))
                .ToList();

            if (!apprenticeships.Any())
            {
                return new GetEmailOverlappingApprenticeshipsQueryResponse
                {
                    Overlaps = Enumerable.Empty<ApprenticeshipEmailOverlapValidationResult>()
                };
            }

            return new GetEmailOverlappingApprenticeshipsQueryResponse
            {
                Overlaps = await _validationApi
               .ValidateEmailOverlapping(apprenticeships.Select(arg => new ApprenticeshipEmailOverlapValidationRequest
               {
                   ApprenticeshipId = arg.Id,
                   Email = arg.Email,
                   StartDate = arg.StartDate.Value,
                   EndDate = arg.EndDate.Value
               }))
            };          
        }
    }
}
