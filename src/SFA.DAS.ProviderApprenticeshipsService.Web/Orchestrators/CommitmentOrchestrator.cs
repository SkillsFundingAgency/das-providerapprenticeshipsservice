using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitments;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.Tasks.Api.Types.Templates;
using TrainingType = SFA.DAS.Commitments.Api.Types.TrainingType;

using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class CommitmentOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ICommitmentStatusCalculator _statusCalculator;

        private readonly IHashingService _hashingService;

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public CommitmentOrchestrator(IMediator mediator, ICommitmentStatusCalculator statusCalculator, IHashingService hashingService)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (statusCalculator == null)
                throw new ArgumentNullException(nameof(statusCalculator));

            _mediator = mediator;
            _statusCalculator = statusCalculator;
            _hashingService = hashingService;
        }

        public async Task<CommitmentListViewModel> GetAll(long providerId)
        {
            Logger.Info($"Getting all commitments for provider:{providerId}");

            var data = await _mediator.SendAsync(new GetCommitmentsQueryRequest
            {
                ProviderId = providerId
            });

            return new CommitmentListViewModel
            {
                ProviderId = providerId,
                Commitments = MapFrom(data.Commitments)
            };
        }

        public async Task<CommitmentDetailsViewModel> GetCommitmentDetails(long providerId, string hashedCommitmentId)
        {
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            Logger.Info($"Getting commitment details:{commitmentId} for provider:{providerId}");

            var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                ProviderId = providerId,
                CommitmentId = commitmentId
            });

            var message = await GetLatestMessage(providerId, commitmentId);

            var apprenticeships = MapFrom(data.Commitment.Apprenticeships);

            return new CommitmentDetailsViewModel
            {
                ProviderId = providerId,
                HashedCommitmentId = hashedCommitmentId,
                LegalEntityName = data.Commitment.LegalEntityName,
                Reference = data.Commitment.Reference,
                Status = _statusCalculator.GetStatus(data.Commitment.EditStatus, data.Commitment.Apprenticeships.Count, data.Commitment.LastAction, data.Commitment.AgreementStatus),
                HasApprenticeships = apprenticeships.Count > 0,
                Apprenticeships = apprenticeships,
                LatestMessage = message,
                PendingChanges = data.Commitment.AgreementStatus != AgreementStatus.EmployerAgreed
            };
        }

        public async Task<CommitmentListItemViewModel> GetCommitment(long providerId, string hashedCommitmentId)
        {
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);

            Logger.Info($"Getting commitment:{commitmentId} for provider:{providerId}");
            var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                ProviderId = providerId,
                CommitmentId = commitmentId
            });

            return MapFrom(data.Commitment);
        }

        public async Task<ExtendedApprenticeshipViewModel> GetApprenticeship(long providerId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);
            Logger.Info($"Getting apprenticeship:{apprenticeshipId} for provider:{providerId}");

            var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
            {
                ProviderId = providerId,
                AppenticeshipId = apprenticeshipId
            });

            var apprenticeship = MapFrom(data.Apprenticeship);

            apprenticeship.ProviderId = providerId;

            var warningValidator = new ApprenticeshipViewModelApproveValidator();
            return new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = apprenticeship,
                ApprenticeshipProgrammes = await GetTrainingProgrammes(),
                WarningValidation = warningValidator.Validate(apprenticeship)
            };
        }

        public async Task<ExtendedApprenticeshipViewModel> GetCreateApprenticeshipViewModel(long providerId, string hashedCommitmentId)
        {
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            Logger.Info($"Getting info for creating apprenticeship for provider:{providerId} commitment:{commitmentId}");

            var apprenticeship = new ApprenticeshipViewModel
            {
                ProviderId = providerId,
                HashedCommitmentId = hashedCommitmentId
            };

            return new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = apprenticeship,
                ApprenticeshipProgrammes = await GetTrainingProgrammes()
            };
        }

        public async Task CreateApprenticeship(ApprenticeshipViewModel apprenticeshipViewModel)
        {
            var apprenticeship = await MapFrom(apprenticeshipViewModel);
            await _mediator.SendAsync(new CreateApprenticeshipCommand
            {
                ProviderId = apprenticeshipViewModel.ProviderId,
                Apprenticeship = apprenticeship
            });

            Logger.Info($"Created apprenticeship for provider:{apprenticeshipViewModel.ProviderId} commitment:{apprenticeship.CommitmentId}");
        }

        public async Task UpdateApprenticeship(ApprenticeshipViewModel apprenticeshipViewModel)
        {
            var apprenticeship = await MapFrom(apprenticeshipViewModel);
            await _mediator.SendAsync(new UpdateApprenticeshipCommand
            {
                ProviderId = apprenticeshipViewModel.ProviderId,
                Apprenticeship = apprenticeship
            });

            Logger.Info($"Updated apprenticeship for provider:{apprenticeshipViewModel.ProviderId} commitment:{apprenticeship.CommitmentId}");
        }

        public async Task SubmitCommitment(long providerId, string hashedCommitmentId, SaveStatus saveStatus, string message)
        {
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            Logger.Info($"Submitting ({saveStatus}) Commitment for provider:{providerId} commitment:{commitmentId}");

            if (saveStatus != SaveStatus.Save)
            {
                var lastAction = saveStatus == SaveStatus.AmendAndSend ? LastAction.Amend : LastAction.Approve;

                await
                    _mediator.SendAsync(
                        new SubmitCommitmentCommand
                        {
                            ProviderId = providerId,
                            CommitmentId = commitmentId,
                            Message = message,
                            LastAction = lastAction,
                            CreateTask = saveStatus != SaveStatus.Approve
                        });
            }
            else
            {
                Logger.Warn($"Commitment submited with illegal state, Save Status: {saveStatus}");
            }
        }

        public async Task<FinishEditingViewModel> GetFinishEditing(long providerId, string hashedCommitmentId)
        {
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);

            Logger.Info($"Get info for finish editing options for provider:{providerId} commitment:{commitmentId}");

            var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                ProviderId = providerId,
                CommitmentId = commitmentId
            });

            AssertCommitmentStatus(data.Commitment, EditStatus.ProviderOnly);
            AssertCommitmentStatus(data.Commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed);

            return new FinishEditingViewModel
            {
                HashedCommitmentId = hashedCommitmentId,
                ProviderId = providerId,
                NotReadyForApproval = !data.Commitment.CanBeApproved,
                ApprovalState = GetApprovalState(data.Commitment),
                HasApprenticeships = data.Commitment.Apprenticeships.Any(),
                InvalidApprenticeshipCount = data.Commitment.Apprenticeships.Count(x => !x.CanBeApproved)
            };
        }

        private static ApprovalState GetApprovalState(Commitment commitment)
        {
            if (!commitment.Apprenticeships.Any()) return ApprovalState.ApproveAndSend;

            var approvalState = commitment.Apprenticeships.Any(m => m.AgreementStatus == AgreementStatus.NotAgreed
                                   || m.AgreementStatus == AgreementStatus.ProviderAgreed) ? ApprovalState.ApproveAndSend : ApprovalState.ApproveOnly;

            return approvalState;
        }

        private IList<ApprenticeshipListItemViewModel> MapFrom(IEnumerable<Apprenticeship> apprenticeships)
        {
            var apprenticeViewModels = apprenticeships.Select(x => new ApprenticeshipListItemViewModel
            {
                HashedApprenticeshipId = _hashingService.HashValue(x.Id),
                ApprenticeshipName = x.ApprenticeshipName,
                ULN = x.ULN,
                TrainingName = x.TrainingName,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Cost = x.Cost,
                CanBeApprove = x.CanBeApproved
            }).ToList();

            return apprenticeViewModels;
        }

        private async Task<string> GetLatestMessage(long providerId, long commitmentId)
        {
            var allTasks = await _mediator.SendAsync(new GetTasksQueryRequest {ProviderId = providerId});

            var taskForCommitment = allTasks?.Tasks
                .Select(x => new {Task = JsonConvert.DeserializeObject<CreateCommitmentTemplate>(x.Body), CreateDate = x.CreatedOn})
                .Where(x => x.Task != null && x.Task.CommitmentId == commitmentId)
                .OrderByDescending(x => x.CreateDate)
                .FirstOrDefault();

            var message = taskForCommitment?.Task?.Message ?? string.Empty;

            return message;
        }

        // TODO: Move mappers into own class
        private List<CommitmentListItemViewModel> MapFrom(List<CommitmentListItem> commitments)
        {
            var commitmentsList = commitments.Select(x => MapFrom(x)).ToList();

            return commitmentsList;
        }

        private CommitmentListItemViewModel MapFrom(CommitmentListItem listItem)
        {
            return new CommitmentListItemViewModel
            {
                HashedCommitmentId = _hashingService.HashValue(listItem.Id),
                Reference = listItem.Reference,
                LegalEntityName = listItem.LegalEntityName,
                ProviderName = listItem.ProviderName,
                Status = _statusCalculator.GetStatus(listItem.EditStatus, listItem.ApprenticeshipCount, listItem.LastAction, listItem.AgreementStatus),
                ShowViewLink = listItem.EditStatus == EditStatus.ProviderOnly
            };
        }

        private CommitmentListItemViewModel MapFrom(Commitment listItem)
        {
            return new CommitmentListItemViewModel
            {
                HashedCommitmentId = _hashingService.HashValue(listItem.Id),
                Reference = listItem.Reference,
                LegalEntityName = listItem.LegalEntityName,
                ProviderName = listItem.ProviderName,
                Status = _statusCalculator.GetStatus(listItem.EditStatus, listItem.Apprenticeships.Count, listItem.LastAction, listItem.AgreementStatus),
                ShowViewLink = listItem.EditStatus == EditStatus.ProviderOnly
            };
        }

        private ApprenticeshipViewModel MapFrom(Apprenticeship apprenticeship)
        {
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
                TrainingCode = apprenticeship.TrainingCode,
                Cost = NullableDecimalToString(apprenticeship.Cost),
                StartDate = new DateTimeViewModel(apprenticeship.StartDate),
                EndDate = new DateTimeViewModel(apprenticeship.EndDate),
                PaymentStatus = apprenticeship.PaymentStatus,
                AgreementStatus = apprenticeship.AgreementStatus,
                ProviderRef = apprenticeship.ProviderRef,
                EmployerRef = apprenticeship.EmployerRef
            };
        }

        private static string NullableDecimalToString(decimal? item)
        {
            return (item.HasValue) ? string.Format("{0:#}", item.Value) : "";
        }

        private async Task<Apprenticeship> MapFrom(ApprenticeshipViewModel viewModel)
        {
            var hashedApprenticeshipId = string.IsNullOrEmpty(viewModel.HashedApprenticeshipId) ?
                0
                : _hashingService.DecodeValue(viewModel.HashedApprenticeshipId);

            var apprenticeship = new Apprenticeship
            {
                Id = hashedApprenticeshipId,
                CommitmentId = _hashingService.DecodeValue(viewModel.HashedCommitmentId),
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                DateOfBirth = viewModel.DateOfBirth.DateTime,
                NINumber = viewModel.NINumber,
                ULN = viewModel.ULN,
                Cost = viewModel.Cost == null ? default(decimal?) : decimal.Parse(viewModel.Cost),
                StartDate = viewModel.StartDate.DateTime,
                EndDate = viewModel.EndDate.DateTime,
                ProviderRef = viewModel.ProviderRef,
                EmployerRef = viewModel.EmployerRef
            };

            if (!string.IsNullOrWhiteSpace(viewModel.TrainingCode))
            {
                var training = await GetTrainingProgramme(viewModel.TrainingCode);
                apprenticeship.TrainingType = training is Standard ? TrainingType.Standard : TrainingType.Framework;
                apprenticeship.TrainingCode = viewModel.TrainingCode;
                apprenticeship.TrainingName = training.Title;
            }

            return apprenticeship;
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

            return
                standardsTask.Result.Standards.Cast<ITrainingProgramme>()
                    .Union(frameworksTask.Result.Frameworks.Cast<ITrainingProgramme>())
                    .OrderBy(m => m.Title)
                    .ToList();
        }

        private static void AssertCommitmentStatus(Commitment commitment, params AgreementStatus[] allowedAgreementStatuses)
        {
            if (commitment == null)
                throw new InvalidStateException("Null commitment");

            if (!allowedAgreementStatuses.Contains(commitment.AgreementStatus))
                throw new InvalidStateException($"Invalid commitment state (agreement status is {commitment.AgreementStatus}, expected {string.Join(",", allowedAgreementStatuses)})");
        }

        private static void AssertCommitmentStatus(Commitment commitment, params EditStatus[] allowedEditStatuses)
        {
            if (commitment == null)
                throw new InvalidStateException("Null commitment");

            if (!allowedEditStatuses.Contains(commitment.EditStatus))
                throw new InvalidStateException($"Invalid commitment state (edit status is {commitment.EditStatus}, expected {string.Join(",", allowedEditStatuses)})");
        }
    }
}
