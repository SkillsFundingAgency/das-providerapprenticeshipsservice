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

            //return GetTestData(request);
        }

        private GetApprenticeshipDataLockResponse GetTestData(GetApprenticeshipDataLockRequest request)
        {
            DataLockStatus dl = null;
            if (request.ApprenticeshipId == 495670)
            {
                dl = new DataLockStatus
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    DataLockEventDatetime = DateTime.Now,
                    DataLockEventId = 123456L,
                    IlrActualStartDate = DateTime.Now,
                    IlrEffectiveFromDate = DateTime.Now,
                    IlrTotalCost = 3000,
                    IlrTrainingCourseCode = "7",
                    IlrTrainingType = Commitments.Api.Types.Apprenticeship.Types.TrainingType.Framework,
                    PriceEpisodeIdentifier = "price-epi",
                    Status = Status.Fail,
                    TriageStatus = TriageStatus.Restart,
                    ErrorCode = DataLockErrorCode.Dlock04 & DataLockErrorCode.Dlock05,
                };

                // Create new
            }

            if (request.ApprenticeshipId == 505672)
            {
                dl = new DataLockStatus
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    DataLockEventDatetime = DateTime.Now,
                    DataLockEventId = 123456L,
                    IlrActualStartDate = DateTime.Now,
                    IlrEffectiveFromDate = DateTime.Now,
                    IlrTotalCost = 3000,
                    IlrTrainingCourseCode = "8",
                    IlrTrainingType = Commitments.Api.Types.Apprenticeship.Types.TrainingType.Framework,
                    PriceEpisodeIdentifier = "price-epi",
                    Status = Status.Fail,
                    TriageStatus = TriageStatus.Change,
                    ErrorCode = DataLockErrorCode.Dlock07 & DataLockErrorCode.Dlock09
                };
                // Fix errors
            }

            var response = new GetApprenticeshipDataLockResponse { Data = dl };

            return response;
        }
    }
}