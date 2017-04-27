using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.DataLock;
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
using TrainingType = SFA.DAS.ProviderApprenticeshipsService.Domain.TrainingType;
using TriageStatus = SFA.DAS.Commitments.Api.Types.DataLock.Types.TriageStatus;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public class ApprenticeshipMapper : IApprenticeshipMapper
    {
        private readonly IHashingService _hashingService;
        private readonly IMediator _mediator;

        public ApprenticeshipMapper(IHashingService hashingService, IMediator mediator)
        {
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));
            if(mediator==null)
                throw new ArgumentNullException(nameof(mediator));

            _hashingService = hashingService;
            _mediator = mediator;
        }

        public ApprenticeshipViewModel MapToApprenticeshipViewModel(Apprenticeship apprenticeship)
        {
            var isStartDateInFuture = apprenticeship.StartDate.HasValue && apprenticeship.StartDate.Value >
                                      new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

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
                EndDate = new DateTimeViewModel(apprenticeship.EndDate),
                PaymentStatus = apprenticeship.PaymentStatus,
                AgreementStatus = apprenticeship.AgreementStatus,
                ProviderRef = apprenticeship.ProviderRef,
                EmployerRef = apprenticeship.EmployerRef,
                HasStarted = !isStartDateInFuture
            };
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

        public ApprenticeshipUpdate MapFrom(ApprenticeshipUpdateViewModel viewModel)
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
                TrainingType = (Commitments.Api.Types.Apprenticeship.Types.TrainingType?) viewModel.TrainingType,
                ProviderRef = viewModel.ProviderRef
            };
        }

        public Apprenticeship MapFromApprenticeshipViewModel(ApprenticeshipViewModel model)
        {
            var id = _hashingService.DecodeValue(model.HashedApprenticeshipId);

            return new Apprenticeship   
            {
                Id = id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth.DateTime,
                NINumber = model.NINumber,
                ULN = model.ULN,
                TrainingType = model.TrainingType,
                TrainingCode = model.TrainingCode,
                Cost = model.Cost.AsNullableDecimal(),
                StartDate = model.StartDate.DateTime,
                EndDate = model.EndDate.DateTime,
                PaymentStatus = model.PaymentStatus,
                AgreementStatus = model.AgreementStatus,
                ProviderRef = model.ProviderRef,
                EmployerRef = model.EmployerRef
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
                    ? (TrainingType) update.TrainingType.Value
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
                ProviderRef = changedOrNull(original.ProviderRef, edited.ProviderRef),
                OriginalApprenticeship = original,
                ProviderName = original.ProviderName,
                LegalEntityName = original.LegalEntityName
            };

            if (!string.IsNullOrWhiteSpace(edited.TrainingCode) && original.TrainingCode != edited.TrainingCode)
            {
                var training = await GetTrainingProgramme(edited.TrainingCode);
                model.TrainingType = training is Standard ? TrainingType.Standard : TrainingType.Framework;
                model.TrainingCode = edited.TrainingCode;
                model.TrainingName = training.Title;
            }

            return model;
        }

        public ApprenticeshipDetailsViewModel MapFrom(Apprenticeship apprenticeship)
        {
            var statusText = MapPaymentStatus(apprenticeship.PaymentStatus, apprenticeship.StartDate);

            var pendingChange = PendingChanges.None;
            if (apprenticeship.PendingUpdateOriginator == Originator.Employer)
                pendingChange = PendingChanges.ReadyForApproval;
            if (apprenticeship.PendingUpdateOriginator == Originator.Provider)
                pendingChange = PendingChanges.WaitingForEmployer;

            var cohortReference = _hashingService.HashValue(apprenticeship.CommitmentId);

            // ToDo: DCM-475 -> Set property on Apprenticeship object
            var hasDataLockErrors = apprenticeship.ULN.StartsWith("11122244");
            return new ApprenticeshipDetailsViewModel
            {
                HashedApprenticeshipId = _hashingService.HashValue(apprenticeship.Id),
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                DateOfBirth = apprenticeship.DateOfBirth,
                Uln = apprenticeship.ULN,
                StartDate = apprenticeship.StartDate,
                EndDate = apprenticeship.EndDate,
                TrainingName = apprenticeship.TrainingName,
                Cost = apprenticeship.Cost,
                Status = statusText,
                EmployerName = apprenticeship.LegalEntityName,
                PendingChanges = pendingChange,
                RecordStatus = MapRecordStatus(apprenticeship .PendingUpdateOriginator, hasDataLockErrors),
                DataLockStatus = MapDataLockStatus(apprenticeship.DataLockTriageStatus),
                CohortReference = cohortReference,
                ProviderReference = apprenticeship.ProviderRef,
                EnableEdit =   pendingChange == PendingChanges.None
                            && apprenticeship.DataLockTriageStatus != TriageStatus.Unknown
                            && new[] { PaymentStatus.Active, PaymentStatus.Paused, }.Contains(apprenticeship.PaymentStatus),
                HasDataLockError = hasDataLockErrors,
                ErrorType = apprenticeship.ULN == "1112224402" 
                    ? DataLockErrorType.UpdateNeeded 
                    : DataLockErrorType.RestartRequire
                // ToDo: DCM-475 -> disable edit if any ILR pending
                            
            };
        }

        public async Task<DataLockViewModel> MapFrom(DataLockStatus dataLock)
        {
            var training = await GetTrainingProgramme(dataLock.IlrTrainingCourseCode);
            return new DataLockViewModel
            {
                DataLockEventId = dataLock.DataLockEventId,
                DataLockEventDatetime = dataLock.DataLockEventDatetime,
                PriceEpisodeIdentifier = dataLock.PriceEpisodeIdentifier,
                ApprenticeshipId = dataLock.ApprenticeshipId,
                IlrTrainingCourseCode = dataLock.IlrTrainingCourseCode,
                IlrTrainingType = (TrainingType)dataLock.IlrTrainingType,
                IlrTrainingCourseName = training.Title,
                IlrActualStartDate = dataLock.IlrActualStartDate,
                IlrEffectiveFromDate = dataLock.IlrEffectiveFromDate,
                IlrTotalCost = dataLock.IlrTotalCost,
                TriageStatusViewModel = (TriageStatusViewModel)dataLock.TriageStatus
            };
        }

        public TriageStatus MapTriangeStatus(SubmitStatusViewModel submitStatusViewModel)
        {
            if (submitStatusViewModel == SubmitStatusViewModel.Confirm)
                return TriageStatus.Change;
            if (submitStatusViewModel == SubmitStatusViewModel.UpdateDataInIlr)
                return TriageStatus.FixIlr;

            return TriageStatus.Unknown;
        }

        private string MapDataLockStatus(TriageStatus dataLockTriageStatus)
        {
            switch (dataLockTriageStatus)
            {
                case TriageStatus.Unknown:
                    break;
                case TriageStatus.Change:
                    return "ILR data mismatch";
                case TriageStatus.Restart:
                    return "Change requested";
                case TriageStatus.FixIlr:
                    return "ILR changes pending";
            }
            return "";
        }

        private string MapRecordStatus(Originator? pendingUpdateOriginator, bool hasDataLockErrors)
        {
            // ToDo: DCM-475 -> PendingUpdate will always take priority over IRL changes -> Unit test
            //if (pendingUpdateOriginator == null)
            //{
            //    if (hasDataLockErrors) return "ILR data mismatch";
            //    // ToDo: DCM-475 -> Check for "ILR data mismatch" or "ILR changes pending"
            //    return string.Empty;
            //}
            if (pendingUpdateOriginator == null)
            {
                return string.Empty;
            }

            return pendingUpdateOriginator == Originator.Provider ? "Changes pending" : "Changes for review";
        }

        private string MapPaymentStatus(PaymentStatus paymentStatus, DateTime? startDate)
        {
            var isStartDateInFuture = startDate.HasValue && startDate.Value > new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            switch (paymentStatus)
            {
                case PaymentStatus.PendingApproval:
                    return "Approval needed";
                case PaymentStatus.Active:
                    return isStartDateInFuture ? "Waiting to start" : "On programme";
                case PaymentStatus.Paused:
                    return "Paused";
                case PaymentStatus.Withdrawn:
                    return "Stopped";
                case PaymentStatus.Completed:
                    return "Completed";
                case PaymentStatus.Deleted:
                    return "Deleted";
                default:
                    return string.Empty;
            }
        }

        private async Task<ITrainingProgramme> GetTrainingProgramme(string trainingCode)
        {
            return (await GetTrainingProgrammes()).Single(x => x.Id == trainingCode);
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