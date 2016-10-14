using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards
{
    public class GetStandardsQueryHandler : IAsyncRequestHandler<GetStandardsQueryRequest, GetStandardsQueryResponse>
    {
        private readonly IApprenticeshipInfoServiceWrapper _apprenticeshipInfoServiceWrapper;

        public GetStandardsQueryHandler(IApprenticeshipInfoServiceWrapper apprenticeshipInfoServiceWrapper)
        {
            if (apprenticeshipInfoServiceWrapper == null)
                throw new ArgumentNullException(nameof(apprenticeshipInfoServiceWrapper));
            _apprenticeshipInfoServiceWrapper = apprenticeshipInfoServiceWrapper;
        }

        public async Task<GetStandardsQueryResponse> Handle(GetStandardsQueryRequest message)
        {
            var standards = await _apprenticeshipInfoServiceWrapper.GetStandardsAsync("Standards");

            return new GetStandardsQueryResponse
            {
                Standards = standards.Standards
            };
        }
    }
}