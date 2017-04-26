using System;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLock
{
    public class GetApprenticeshipDataLockHandler :
        IAsyncRequestHandler<GetApprenticeshipDataLockRequest, GetApprenticeshipDataLockResponse>
    {
        public Task<GetApprenticeshipDataLockResponse> Handle(GetApprenticeshipDataLockRequest message)
        {
            DataLockStatus dl = null;
            if (message.ApprenticeshipId == 495670)
            {
                dl = new DataLockStatus
                {
                    ApprenticeshipId = message.ApprenticeshipId,
                    DataLockEventDatetime = DateTime.Now,
                    DataLockEventId = 123456L,
                    IlrActualStartDate = DateTime.Now,
                    IlrEffectiveFromDate = DateTime.Now,
                    IlrTotalCost = 3000,
                    IlrTrainingCourseCode = "123-123-123",
                    IlrTrainingType = TrainingType.Framework,
                    PriceEpisodeIdentifier = "price-epi",
                    Status = Status.Fail,
                    TriageStatus = TriageStatus.Restart,
                    ErrorCode = DataLockErrorCode.Dlock04 & DataLockErrorCode.Dlock05
                };

                // Create new
            }

            if (message.ApprenticeshipId == 505672)
            {
                dl = new DataLockStatus
                {
                    ApprenticeshipId = message.ApprenticeshipId,
                    DataLockEventDatetime = DateTime.Now,
                    DataLockEventId = 123456L,
                    IlrActualStartDate = DateTime.Now,
                    IlrEffectiveFromDate = DateTime.Now,
                    IlrTotalCost = 3000,
                    IlrTrainingCourseCode = "123-123-123",
                    IlrTrainingType = TrainingType.Framework,
                    PriceEpisodeIdentifier = "price-epi",
                    Status = Status.Fail,
                    TriageStatus = TriageStatus.Change,
                    ErrorCode = DataLockErrorCode.Dlock07 & DataLockErrorCode.Dlock09
                };
                // Fix errors
            }

            var response = new GetApprenticeshipDataLockResponse { Data = dl };

            return Task.Run(() => response);
        }
    }
}