﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
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
using SFA.DAS.ProviderApprenticeshipsService.Application.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.AcademicYear;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

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

            var isUpdateLockedForStartDateAndCourse = false;
            var isEndDateLockedForUpdate = false;
            var isLockedForUpdate = false;

            //locking fields concerns post-approval apprenticeships only
            //this method should be split into pre- and post-approval versions
            //ideally dealing with different api types and view models
            if (apprenticeship.PaymentStatus != PaymentStatus.PendingApproval)
            {
                isLockedForUpdate = (!isStartDateInFuture &&
                                         (apprenticeship.HasHadDataLockSuccess || _academicYearValidator.IsAfterLastAcademicYearFundingPeriod &&
                                          apprenticeship.StartDate.HasValue &&
                                          _academicYearValidator.Validate(apprenticeship.StartDate.Value) == AcademicYearValidationResult.NotWithinFundingPeriod))
                                        ||
                                        (commitment.TransferSender?.TransferApprovalStatus == TransferApprovalStatus.Approved
                                         && apprenticeship.HasHadDataLockSuccess && isStartDateInFuture);

                isUpdateLockedForStartDateAndCourse =
                    commitment.TransferSender?.TransferApprovalStatus == TransferApprovalStatus.Approved
                    && !apprenticeship.HasHadDataLockSuccess;

                // if editing post-approval, we also lock down end date if...
                //   start date is in the future and has had data lock success
                //   (as the validation rule that disallows setting end date to > current month
                //   means any date entered would be before the start date (which is also disallowed))
                // and open it up if...
                //   data lock success and start date in past
                isEndDateLockedForUpdate = isLockedForUpdate;
                if (commitment.AgreementStatus == AgreementStatus.BothAgreed
                    && apprenticeship.HasHadDataLockSuccess)
                {
                    isEndDateLockedForUpdate = isStartDateInFuture;
                }

            }


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
                CourseType = apprenticeship.TrainingType,
                CourseName = apprenticeship.TrainingName,
                CourseCode = apprenticeship.TrainingCode,
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
                IsPaidForByTransfer = commitment.IsTransfer(),
                IsUpdateLockedForStartDateAndCourse = isUpdateLockedForStartDateAndCourse,
                IsEndDateLockedForUpdate = isEndDateLockedForUpdate,
                ReservationId = apprenticeship.ReservationId,
                IsContinuation = apprenticeship.ContinuationOfId.HasValue
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
                EmployerRef = vm.EmployerRef,
                ReservationId = vm.ReservationId
            };

            if (!string.IsNullOrWhiteSpace(vm.CourseCode))
            {
                var training = await GetTrainingProgramme(vm.CourseCode);

                if (training != null)
                {
                    apprenticeship.TrainingType = int.TryParse(training.CourseCode, out _) ? CommitmentTrainingType.Standard : CommitmentTrainingType.Framework;
                    apprenticeship.TrainingCode = vm.CourseCode;
                    apprenticeship.TrainingName = training.Name;
                }
                else
                {
                    apprenticeship.TrainingType = vm.CourseType;
                    apprenticeship.TrainingCode = vm.CourseCode;
                    apprenticeship.TrainingName = vm.CourseName;

                    _logger.Warn($"Apprentice training course has expired. TrainingName: {apprenticeship.TrainingName}, TrainingCode: {apprenticeship.TrainingCode}, Employer Ref: {apprenticeship.EmployerRef}, ApprenticeshipId: {apprenticeship.Id}, Apprenticeship ULN: {apprenticeship.ULN}");
                }
            }

            return apprenticeship;
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
                TrainingName = viewModel.CourseName,
                TrainingCode = viewModel.CourseCode,
                TrainingType = (Commitments.Api.Types.Apprenticeship.Types.TrainingType?)viewModel.CourseType,
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
                CourseType = update.TrainingType.HasValue
                    ? (TrainingType)update.TrainingType.Value
                    : default(TrainingType?),
                CourseCode = update.TrainingCode,
                CourseName = update.TrainingName,
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
            string ChangedOrNull(string a, string edit) => a?.Trim() == edit?.Trim() ? null : edit;

            var model = new CreateApprenticeshipUpdateViewModel
            {
                HashedApprenticeshipId = _hashingService.HashValue(original.Id),
                ULN = ChangedOrNull(original.ULN, edited.ULN),
                FirstName = ChangedOrNull(original.FirstName, edited.FirstName),
                LastName = ChangedOrNull(original.LastName, edited.LastName),
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
                LegalEntityName = original.LegalEntityName,
                ReservationId = original.ReservationId
            };

            if (!string.IsNullOrWhiteSpace(edited.CourseCode) && original.TrainingCode != edited.CourseCode)
            {
                var training = await GetTrainingProgramme(edited.CourseCode);

                if (training != null)
                {
                    model.CourseType = int.TryParse(training.CourseCode,out _) ? TrainingType.Standard : TrainingType.Framework;
                    model.CourseCode = edited.CourseCode;
                    model.CourseName = training.Name;
                }
                else
                {
                    model.CourseType = edited.CourseType == CommitmentTrainingType.Standard ? TrainingType.Standard : TrainingType.Framework; 
                    model.CourseCode = edited.CourseCode;
                    model.CourseName = edited.CourseName;

                    _logger.Warn($"Apprentice training course has expired. TrainingName: {edited.CourseName}, TrainingCode: {edited.CourseCode}, Employer Ref: {edited.EmployerRef}, Apprenticeship ULN: {edited.ULN}");
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
                AccountLegalEntityPublicHashedId = apprenticeship.AccountLegalEntityPublicHashedId,
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

        private async Task<TrainingProgramme> GetTrainingProgramme(string trainingCode)
        {
            return (await GetTrainingProgrammes()).FirstOrDefault(x => x.CourseCode == trainingCode);
        }

        private async Task<List<TrainingProgramme>> GetTrainingProgrammes()
        {
            var programmes = await _mediator.Send(new GetTrainingProgrammesQueryRequest
            {
                IncludeFrameworks = true
            });
            return programmes.TrainingProgrammes;
        }

        private static string NullableDecimalToString(decimal? item)
        {
            return (item.HasValue) ? string.Format("{0:#}", item.Value) : "";
        }
    }
}