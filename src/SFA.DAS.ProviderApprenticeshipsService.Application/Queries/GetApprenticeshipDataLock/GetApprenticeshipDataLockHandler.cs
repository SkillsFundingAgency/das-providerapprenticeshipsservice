using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLock
{
    public class GetApprenticeshipDataLockHandler : IAsyncRequestHandler<GetApprenticeshipDataLockRequest, GetApprenticeshipDataLockResponse>
    {
        private readonly IDataLockApi _dataLockApi;

        public GetApprenticeshipDataLockHandler(
            IDataLockApi dataLockApi)
        {
            _dataLockApi = dataLockApi;
        }

        public async Task<GetApprenticeshipDataLockResponse> Handle(GetApprenticeshipDataLockRequest request)
        {
            var data = await _dataLockApi.GetDataLocks(request.ApprenticeshipId);

            return new GetApprenticeshipDataLockResponse
            {
                Data = data.FirstOrDefault(m => m.Status == Status.Fail && !m.IsResolved)
            };
        }
    }
}