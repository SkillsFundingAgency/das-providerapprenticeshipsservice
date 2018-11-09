using System.Linq;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.ApprovedApprenticeship;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public class ApprovedApprenticeshipMapper : IApprovedApprenticeshipMapper
    {
        private readonly IPaymentStatusMapper _paymentStatusMapper;
        private readonly IHashingService _hashingService;
        private readonly IAlertMapper _alertMapper;
        private readonly ICurrentDateTime _currentDateTime;

        public ApprovedApprenticeshipMapper
        (
            IPaymentStatusMapper paymentStatusMapper,
            IHashingService hashingService,
            IAlertMapper alertMapper,
            ICurrentDateTime currentDateTime
        ){
            _paymentStatusMapper = paymentStatusMapper;
            _hashingService = hashingService;
            _alertMapper = alertMapper;
            _currentDateTime = currentDateTime;
        }

        public ApprovedApprenticeshipViewModel Map(ApprovedApprenticeship apprenticeship)
        {
            var statusText = _paymentStatusMapper.Map(apprenticeship.PaymentStatus, apprenticeship.StartDate);

            var pendingChange = PendingChanges.None;
            if (apprenticeship.UpdateOriginator == Originator.Employer)
                pendingChange = PendingChanges.ReadyForApproval;
            if (apprenticeship.UpdateOriginator == Originator.Provider)
                pendingChange = PendingChanges.WaitingForEmployer;

            var model = new ApprovedApprenticeshipViewModel
            {
                HashedApprenticeshipId = _hashingService.HashValue(apprenticeship.Id),
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                DateOfBirth = apprenticeship.DateOfBirth,
                Uln = apprenticeship.ULN,
                StartDate = apprenticeship.StartDate,
                EndDate = apprenticeship.EndDate,
                StopDate = apprenticeship.StopDate,
                TrainingName = apprenticeship.TrainingName,
                Cost = apprenticeship.PriceEpisodes.GetCostOn(_currentDateTime.Now),
                Status = statusText,
                EmployerName = apprenticeship.LegalEntityName,
                PendingChanges = pendingChange,
                CohortReference = apprenticeship.CohortReference,
                AccountLegalEntityPublicHashedId = apprenticeship.AccountLegalEntityPublicHashedId,
                ProviderReference = apprenticeship.ProviderRef,
                HasHadDataLockSuccess = apprenticeship.HasHadDataLockSuccess
            };

            //Statuses
            model.EnableEdit = model.PendingChanges == PendingChanges.None
                               && !model.PendingDataLockChange
                               && !model.PendingDataLockRestart
                               && new[] { PaymentStatus.Active, PaymentStatus.Paused }.Contains(apprenticeship.PaymentStatus);

            model.DataLockCourse =
                apprenticeship.DataLocks.Any(x => x.WithCourseError() && x.TriageStatus == TriageStatus.Unknown);
            model.DataLockPrice =
                apprenticeship.DataLocks.Any(x => x.IsPriceOnly() && x.TriageStatus == TriageStatus.Unknown);
            model.DataLockCourseTriaged =
                apprenticeship.DataLocks.Any(x => x.WithCourseError() && x.TriageStatus == TriageStatus.Restart);
            model.DataLockCourseChangeTriaged =
                apprenticeship.DataLocks.Any(x => x.WithCourseError() && x.TriageStatus == TriageStatus.Change);
            model.DataLockPriceTriaged =
                apprenticeship.DataLocks.Any(x => x.IsPriceOnly() && x.TriageStatus == TriageStatus.Change);

            model.Alerts = _alertMapper.MapAlerts(model, apprenticeship);

            return model;
        }
    }
}