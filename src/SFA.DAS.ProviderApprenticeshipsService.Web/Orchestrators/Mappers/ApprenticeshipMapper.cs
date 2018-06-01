using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.Commitments.Api.Types.Validation.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.NLog.Logger;

using TrainingType = SFA.DAS.ProviderApprenticeshipsService.Domain.TrainingType;
using TriageStatus = SFA.DAS.Commitments.Api.Types.DataLock.Types.TriageStatus;
using CommitmentTrainingType = SFA.DAS.Commitments.Api.Types.Apprenticeship.Types.TrainingType;
using SFA.DAS.HashingService;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public class ApprenticeshipMapper : IApprenticeshipMapper
    {
        private readonly IHashingService _hashingService;
        private readonly IMediator _mediator;
        private readonly ICurrentDateTime _currentDateTime;
        private readonly ILog _logger;
        private readonly IAcademicYearValidator _academicYearValidator;

        public ApprenticeshipMapper(
            IHashingService hashingService, 
            IMediator mediator, 
            ICurrentDateTime currentDateTime, 
            ILog logger,
            IAcademicYearValidator academicYearValidator)
        {
            _hashingService = hashingService;
            _mediator = mediator;
            _currentDateTime = currentDateTime;
            _logger = logger;
            _academicYearValidator = academicYearValidator;
        }

        public ApprenticeshipViewModel MapApprenticeship(Apprenticeship apprenticeship, CommitmentView commitment)
        {
            var isStartDateInFuture = apprenticeship.StartDate.HasValue && apprenticeship.StartDate.Value >
                                      new DateTime(_currentDateTime.Now.Year, _currentDateTime.Now.Month, 1);

            var isLockedForUpdate = (!isStartDateInFuture &&
                                     (apprenticeship.HasHadDataLockSuccess || _academicYearValidator.IsAfterLastAcademicYearFundingPeriod &&
                                      apprenticeship.StartDate.HasValue &&
                                      _academicYearValidator.Validate(apprenticeship.StartDate.Value) == AcademicYearValidationResult.NotWithinFundingPeriod))
                                    ||
                                    (commitment.TransferSender?.TransferApprovalStatus == TransferApprovalStatus.Approved
                                     && apprenticeship.HasHadDataLockSuccess && isStartDateInFuture);

            var isUpdateLockedForStartDateAndCourse =
                commitment.TransferSender?.TransferApprovalStatus == TransferApprovalStatus.Approved
                && !apprenticeship.HasHadDataLockSuccess;

            var dateOfBirth = apprenticeship.DateOfBirth;
            return new ApprenticeshipViewModel
            {
                HashedApprenticeshipId = _hashingService.HashValue(apprenticeship.Id),
                HashedCommitmentId = _hashingService.HashValue(apprenticeship.CommitmentId),
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                DateOfBirth = new DateTimeViewModel(dateOfBirth?.Day, dateOfBirth?.Month, dateOfBirth?.Year),
                NINumber = apprenticeship.NINumber,
                ULN = apprenticeship.ULN,
                TrainingType = apprenticeship.TrainingType,
                TrainingName = apprenticeship.TrainingName,
                TrainingCode = apprenticeship.TrainingCode,
                Cost = NullableDecimalToString(apprenticeship.Cost),
                StartDate = new DateTimeViewModel(apprenticeship.StartDate),
                StopDate = new DateTimeViewModel(apprenticeship.StopDate),
                EndDate = new DateTimeViewModel(apprenticeship.EndDate),
                PaymentStatus = apprenticeship.PaymentStatus,
                AgreementStatus = apprenticeship.AgreementStatus,
                ProviderRef = apprenticeship.ProviderRef,
                EmployerRef = apprenticeship.EmployerRef,
                HasStarted = !isStartDateInFuture,
                IsLockedForUpdate = isLockedForUpdate,
                IsPaidForByTransfer = commitment.TransferSender != null,
                IsUpdateLockedForStartDateAndCourse = isUpdateLockedForStartDateAndCourse
            };
        }

        public async Task<Apprenticeship> MapApprenticeship(ApprenticeshipViewModel vm)
        {
            var id = string.IsNullOrEmpty(vm.HashedApprenticeshipId)
                ? 0
                : _hashingService.DecodeValue(vm.HashedApprenticeshipId);

            var apprenticeship = new Apprenticeship
            {
                Id = id,
                CommitmentId = _hashingService.DecodeValue(vm.HashedCommitmentId),
                ProviderId = vm.ProviderId,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                DateOfBirth = vm.DateOfBirth.DateTime,
                NINumber = vm.NINumber,
                ULN = vm.ULN,
                Cost = vm.Cost.AsNullableDecimal(),
                StartDate = vm.StartDate.DateTime,
                EndDate = vm.EndDate.DateTime,
                PaymentStatus = vm.PaymentStatus,
                AgreementStatus = vm.AgreementStatus,
                ProviderRef = vm.ProviderRef,
                EmployerRef = vm.EmployerRef
            };

            if (!string.IsNullOrWhiteSpace(vm.TrainingCode))
            {
                var training = await GetTrainingProgramme(vm.TrainingCode);

                if (training != null)
                {
                    apprenticeship.TrainingType = (CommitmentTrainingType)(training is Standard ? TrainingType.Standard : TrainingType.Framework);
                    apprenticeship.TrainingCode = vm.TrainingCode;
                    apprenticeship.TrainingName = training.Title;
                }
                else
                {
                    apprenticeship.TrainingType = vm.TrainingType;
                    apprenticeship.TrainingCode = vm.TrainingCode;
                    apprenticeship.TrainingName = vm.TrainingName;

                    _logger.Warn($"Apprentice training course has expired. TrainingName: {apprenticeship.TrainingName}, TrainingCode: {apprenticeship.TrainingCode}, Employer Ref: {apprenticeship.EmployerRef}, ApprenticeshipId: {apprenticeship.Id}, Apprenticeship ULN: {apprenticeship.ULN}");
                }


            }

            return apprenticeship;
        }

        public Dictionary<string, string> MapOverlappingErrors(GetOverlappingApprenticeshipsQueryResponse overlappingErrors)
        {
            var dict = new Dictionary<string, string>();
            const string StartText = "The start date is not valid";
            const string EndText = "The end date is not valid";

            const string StartDateKey = "StartDateOverlap";
            const string EndDateKey = "EndDateOverlap";


            foreach (var item in overlappingErrors.GetFirstOverlappingApprenticeships())
            {
                switch (item.ValidationFailReason)
                {
                    case ValidationFailReason.OverlappingStartDate:
                        dict.AddIfNotExists(StartDateKey, StartText);
                        break;
                    case ValidationFailReason.OverlappingEndDate:
                        dict.AddIfNotExists(EndDateKey, EndText);
                        break;
                    case ValidationFailReason.DateEmbrace:
                        dict.AddIfNotExists(StartDateKey, StartText);
                        dict.AddIfNotExists(EndDateKey, EndText);
                        break;
                    case ValidationFailReason.DateWithin:
                        dict.AddIfNotExists(StartDateKey, StartText);
                        dict.AddIfNotExists(EndDateKey, EndText);
                        break;
                }
            }
            return dict;
        }

        public ApprenticeshipUpdate MapApprenticeshipUpdate(ApprenticeshipUpdateViewModel viewModel)
        {
            return new ApprenticeshipUpdate
            {
                ApprenticeshipId = viewModel.OriginalApprenticeship.Id,
                ULN = viewModel.ULN,
                Cost = viewModel.Cost.AsNullableDecimal(),
                DateOfBirth = viewModel.DateOfBirth?.DateTime,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                StartDate = viewModel.StartDate?.DateTime,
                EndDate = viewModel.EndDate?.DateTime,
                Originator = Originator.Provider,
                Status = ApprenticeshipUpdateStatus.Pending,
                TrainingName = viewModel.TrainingName,
                TrainingCode = viewModel.TrainingCode,
                TrainingType = (Commitments.Api.Types.Apprenticeship.Types.TrainingType?)viewModel.TrainingType,
                ProviderRef = viewModel.ProviderRef
            };
        }

        public T MapApprenticeshipUpdateViewModel<T>(Apprenticeship original, ApprenticeshipUpdate update) where T : ApprenticeshipUpdateViewModel, new()
        {
            return new T
            {
                HashedApprenticeshipId = _hashingService.HashValue(update.ApprenticeshipId),
                FirstName = update.FirstName,
                LastName = update.LastName,
                DateOfBirth = new DateTimeViewModel(update.DateOfBirth),
                ULN = update.ULN,
                TrainingType = update.TrainingType.HasValue
                    ? (TrainingType)update.TrainingType.Value
                    : default(TrainingType?),
                TrainingCode = update.TrainingCode,
                TrainingName = update.TrainingName,
                Cost = update.Cost.HasValue ? update.Cost.ToString() : string.Empty,
                StartDate = new DateTimeViewModel(update.StartDate),
                EndDate = new DateTimeViewModel(update.EndDate),
                ProviderRef = update.ProviderRef,
                EmployerRef = update.EmployerRef,
                OriginalApprenticeship = original,
                LegalEntityName = original.LegalEntityName,
                ProviderName = original.ProviderName
            };
        }

        public async Task<CreateApprenticeshipUpdateViewModel> CompareAndMapToCreateUpdateApprenticeshipViewModel(Apprenticeship original, ApprenticeshipViewModel edited)
        {
            Func<string, string, string> changedOrNull = (a, edit) =>
               a?.Trim() == edit?.Trim() ? null : edit;

            var model = new CreateApprenticeshipUpdateViewModel
            {
                HashedApprenticeshipId = _hashingService.HashValue(original.Id),
                ULN = changedOrNull(original.ULN, edited.ULN),
                FirstName = changedOrNull(original.FirstName, edited.FirstName),
                LastName = changedOrNull(original.LastName, edited.LastName),
                DateOfBirth = original.DateOfBirth == edited.DateOfBirth.DateTime
                    ? null
                    : edited.DateOfBirth,
                Cost = original.Cost == edited.Cost.AsNullableDecimal() ? null : edited.Cost,
                StartDate = original.StartDate == edited.StartDate.DateTime
                  ? null
                  : edited.StartDate,
                EndDate = original.EndDate == edited.EndDate.DateTime
                    ? null
                    : edited.EndDate,
                ProviderRef = original.ProviderRef?.Trim() == edited.ProviderRef?.Trim()
                            || (string.IsNullOrEmpty(original.ProviderRef) && string.IsNullOrEmpty(edited.ProviderRef))
                    ? null
                    : edited.ProviderRef ?? "",
                OriginalApprenticeship = original,
                ProviderName = original.ProviderName,
                LegalEntityName = original.LegalEntityName
            };

            if (!string.IsNullOrWhiteSpace(edited.TrainingCode) && original.TrainingCode != edited.TrainingCode)
            {
                var training = await GetTrainingProgramme(edited.TrainingCode);

                if (training != null)
                {
                    model.TrainingType = training is Standard ? TrainingType.Standard : TrainingType.Framework;
                    model.TrainingCode = edited.TrainingCode;
                    model.TrainingName = training.Title;
                }
                else
                {
                    model.TrainingType = edited.TrainingType == CommitmentTrainingType.Standard ? TrainingType.Standard : TrainingType.Framework; 
                    model.TrainingCode = edited.TrainingCode;
                    model.TrainingName = edited.TrainingName;

                    _logger.Warn($"Apprentice training course has expired. TrainingName: {edited.TrainingName}, TrainingCode: {edited.TrainingCode}, Employer Ref: {edited.EmployerRef}, Apprenticeship ULN: {edited.ULN}");
                }

            }

            return model;
        }

        public ApprenticeshipDetailsViewModel MapApprenticeshipDetails(Apprenticeship apprenticeship)
        {
            var statusText = MapPaymentStatus(apprenticeship.PaymentStatus, apprenticeship.StartDate);

            var pendingChange = PendingChanges.None;
            if (apprenticeship.PendingUpdateOriginator == Originator.Employer)
                pendingChange = PendingChanges.ReadyForApproval;
            if (apprenticeship.PendingUpdateOriginator == Originator.Provider)
                pendingChange = PendingChanges.WaitingForEmployer;

            return new ApprenticeshipDetailsViewModel
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
                Cost = apprenticeship.Cost,
                Status = statusText,
                EmployerName = apprenticeship.LegalEntityName,
                PendingChanges = pendingChange,
                Alerts = MapAlerts(apprenticeship),
                CohortReference = _hashingService.HashValue(apprenticeship.CommitmentId),
                ProviderReference = apprenticeship.ProviderRef,
                HasHadDataLockSuccess = apprenticeship.HasHadDataLockSuccess,
                EnableEdit = pendingChange == PendingChanges.None
                            && !apprenticeship.DataLockCourse
                            && !apprenticeship.DataLockPrice
                            && !apprenticeship.DataLockCourseTriaged
                            && !apprenticeship.DataLockCourseChangeTriaged
                            && !apprenticeship.DataLockPriceTriaged
                            && new[] { PaymentStatus.Active, PaymentStatus.Paused, }.Contains(apprenticeship.PaymentStatus)
            };
        }

        private List<string> MapAlerts(Apprenticeship apprenticeship)
        {
            var result = new List<string>();

            if (apprenticeship.DataLockCourse || apprenticeship.DataLockPrice)
            {
                result.Add("ILR data mismatch");
            }

            if (apprenticeship.DataLockPriceTriaged || apprenticeship.DataLockCourseChangeTriaged)
            {
                result.Add("Changes pending");
            }

            if (apprenticeship.DataLockCourseTriaged)
            {
                result.Add("Changes requested");
            }

            if (apprenticeship.PendingUpdateOriginator != null)
            {
                if (apprenticeship.PendingUpdateOriginator == Originator.Provider)
                {
                    result.Add("Changes pending");
                }
                else
                {
                    result.Add("Changes for review");
                }
            }

            return result.Distinct().ToList();
        }

        public DataLockErrorType MapErrorType(DataLockErrorCode errorCode)
        {
            if (errorCode.HasFlag(DataLockErrorCode.Dlock03)
                || errorCode.HasFlag(DataLockErrorCode.Dlock04)
                || errorCode.HasFlag(DataLockErrorCode.Dlock05)
                || errorCode.HasFlag(DataLockErrorCode.Dlock06)
                )
                return DataLockErrorType.RestartRequired;

            if (errorCode.HasFlag(DataLockErrorCode.Dlock07)
                || errorCode.HasFlag(DataLockErrorCode.Dlock09))
                return DataLockErrorType.UpdateNeeded;

            return DataLockErrorType.None;
        }

        public TriageStatus MapTriangeStatus(SubmitStatusViewModel submitStatusViewModel)
        {
            if (submitStatusViewModel == SubmitStatusViewModel.Confirm)
                return TriageStatus.Change;
            if (submitStatusViewModel == SubmitStatusViewModel.UpdateDataInIlr)
                return TriageStatus.FixIlr;

            return TriageStatus.Unknown;
        }

        private string MapPaymentStatus(PaymentStatus paymentStatus, DateTime? startDate)
        {
            var isStartDateInFuture = startDate.HasValue && startDate.Value > new DateTime(_currentDateTime.Now.Year, _currentDateTime.Now.Month, 1);

            switch (paymentStatus)
            {
                case PaymentStatus.PendingApproval:
                    return "Approval needed";
                case PaymentStatus.Active:
                    return
                        isStartDateInFuture ? "Waiting to start" : "Live";
                case PaymentStatus.Paused:
                    return "Paused";
                case PaymentStatus.Withdrawn:
                    return "Stopped";
                case PaymentStatus.Completed:
                    return "Finished";
                case PaymentStatus.Deleted:
                    return "Deleted";
                default:
                    return string.Empty;
            }
        }

        private async Task<ITrainingProgramme> GetTrainingProgramme(string trainingCode)
        {
            return (await GetTrainingProgrammes()).FirstOrDefault(x => x.Id == trainingCode);
        }

        private async Task<List<ITrainingProgramme>> GetTrainingProgrammes()
        {
            var standardsTask = _mediator.SendAsync(new GetStandardsQueryRequest());
            var frameworksTask = _mediator.SendAsync(new GetFrameworksQueryRequest());

            await Task.WhenAll(standardsTask, frameworksTask);

            return standardsTask.Result.Standards.Cast<ITrainingProgramme>().Union(frameworksTask.Result.Frameworks.Cast<ITrainingProgramme>()).OrderBy(m => m.Title).ToList();
        }

        private static string NullableDecimalToString(decimal? item)
        {
            return (item.HasValue) ? string.Format("{0:#}", item.Value) : "";
        }
    }
}