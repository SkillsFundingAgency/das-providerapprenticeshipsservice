using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLockSummary
{
    public class GetApprenticeshipDataLockSummaryQueryHandler : IAsyncRequestHandler<GetApprenticeshipDataLockSummaryQueryRequest,GetApprenticeshipDataLockSummaryQueryResponse>
    {
        private readonly IDataLockApi _dataLockApi;

        public GetApprenticeshipDataLockSummaryQueryHandler(IDataLockApi dataLockApi)
        {
            _dataLockApi = dataLockApi;
        }

        public async Task<GetApprenticeshipDataLockSummaryQueryResponse> Handle(GetApprenticeshipDataLockSummaryQueryRequest command)
        {
            var response = await _dataLockApi.GetDataLockSummary(command.ApprenticeshipId);

            return new GetApprenticeshipDataLockSummaryQueryResponse
            {
                DataLockSummary = response
            };
        }
    }
}
