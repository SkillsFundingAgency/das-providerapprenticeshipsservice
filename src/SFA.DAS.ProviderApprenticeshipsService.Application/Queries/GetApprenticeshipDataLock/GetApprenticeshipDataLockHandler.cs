using System;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLock
{
    public class GetApprenticeshipDataLockHandler :
        IAsyncRequestHandler<GetApprenticeshipDataLockRequest, GetApprenticeshipDataLockResponse>
    {
        private readonly IDataLockApi _dataLockApi;

        public GetApprenticeshipDataLockHandler(IDataLockApi dataLockApi)
        {
            _dataLockApi = dataLockApi;
        }

        public async Task<GetApprenticeshipDataLockResponse> Handle(GetApprenticeshipDataLockRequest request)
        {
            //var data = await _dataLockApi.GetDataLocks(request.ApprenticeshipId);
            //return new GetApprenticeshipDataLockResponse { Data = data.FirstOrDefault() };

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
                    IlrTrainingCourseCode = "2",
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
                    IlrTrainingCourseCode = "4",
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