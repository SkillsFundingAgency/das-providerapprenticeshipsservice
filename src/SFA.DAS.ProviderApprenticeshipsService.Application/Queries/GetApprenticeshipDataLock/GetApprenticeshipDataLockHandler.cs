using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLock
{
    public class GetApprenticeshipDataLockHandler :
        IAsyncRequestHandler<GetApprenticeshipDataLockRequest, GetApprenticeshipDataLockResponse>
    {
        private readonly IDataLockApi _dataLockApi;

        private readonly ILog _logger;

        public GetApprenticeshipDataLockHandler(
            IDataLockApi dataLockApi,
            ILog logger)
        {
            _dataLockApi = dataLockApi;
            _logger = logger;
        }

        public async Task<GetApprenticeshipDataLockResponse> Handle(GetApprenticeshipDataLockRequest request)
        {
            try
            {
                var data = await _dataLockApi.GetDataLocks(request.ApprenticeshipId);
                return new GetApprenticeshipDataLockResponse { Data = data.FirstOrDefault() };
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, $"Can't get apprenticeship data lock for apprenticeship {request.ApprenticeshipId}");
            }
            return new GetApprenticeshipDataLockResponse();
        }
    }
}