using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLocks
{
    public class GetApprenticeshipDataLocksHandler : IAsyncRequestHandler<GetApprenticeshipDataLocksRequest, GetApprenticeshipDataLocksResponse>
    {
        private readonly IDataLockApi _dataLockApi;

        public GetApprenticeshipDataLocksHandler(
            IDataLockApi dataLockApi)
        {
            _dataLockApi = dataLockApi;
        }

        public async Task<GetApprenticeshipDataLocksResponse> Handle(GetApprenticeshipDataLocksRequest request)
        {
            var data = await _dataLockApi.GetDataLocks(request.ApprenticeshipId);

            return new GetApprenticeshipDataLocksResponse
            {
                Data = data.FindAll(m => m.Status == Status.Fail && !m.IsResolved)
            };
        }
    }
}