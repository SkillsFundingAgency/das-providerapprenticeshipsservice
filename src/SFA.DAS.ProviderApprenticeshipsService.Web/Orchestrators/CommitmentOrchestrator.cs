using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
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
    public class CommitmentOrchestrator : BaseCommitmentOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ICommitmentStatusCalculator _statusCalculator;
        private readonly IHashingService _hashingService;
        private readonly IProviderCommitmentsLogger _logger;

        readonly Func<CommitmentListItem, Task<string>> _latestMessageToEmployerFunc;

        readonly Func<CommitmentListItem, Task<string>> _latestMessageToProviderFunc;

        public CommitmentOrchestrator(IMediator mediator, ICommitmentStatusCalculator statusCalculator, IHashingService hashingService, IProviderCommitmentsLogger logger)
        : base (mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (statusCalculator == null)
                throw new ArgumentNullException(nameof(statusCalculator));
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _mediator = mediator;
            _statusCalculator = statusCalculator;
            _hashingService = hashingService;
            _logger = logger;

            _latestMessageToEmployerFunc = async item => await GetLatestMessage(item.EmployerAccountId, item.Id, false);
            _latestMessageToProviderFunc = async item => await GetLatestMessage(item.ProviderId, item.Id, true);
        }

        public async Task<CohortsViewModel> GetCohorts(long providerId)
        {
            _logger.Info($"Getting cohorts :{providerId}", providerId);
            var data = await _mediator.SendAsync(new GetCommitmentsQueryRequest
            {
                ProviderId = providerId
            });
            var commitmentStatus =
                data.Commitments.Select(m =>
                    _statusCalculator.GetStatus(m.EditStatus, m.ApprenticeshipCount, m.LastAction, m.AgreementStatus)).ToList();

            var model = new CohortsViewModel
            {
                NewRequestsCount = commitmentStatus.Count(m => m == RequestStatus.NewRequest),
                ReadyForApprovalCount = commitmentStatus.Count(m => m == RequestStatus.ReadyForApproval),
                ReadyForReviewCount = commitmentStatus.Count(m => m == RequestStatus.ReadyForReview),
                WithEmployerCount = commitmentStatus.Count(m => m == RequestStatus.SentForReview || m == RequestStatus.WithEmployerForApproval)
            };

            return model;
        }

        public async Task<CommitmentListViewModel> GetAllWithEmployer(long providerId)
        {
            var sentForReview = await GetAll(providerId, RequestStatus.SentForReview);
            var sentForApproval = await GetAll(providerId, RequestStatus.WithEmployerForApproval);
            var data = sentForReview.Concat(sentForApproval).ToList();
            _logger.Info($"Provider getting all with employer ({data.Count}) :{providerId}", providerId);

            return new CommitmentListViewModel
            {
                ProviderId = providerId,
                Commitments = await MapFrom(data, _latestMessageToEmployerFunc),
                PageTitle = "Cohorts with employers",
                PageId = "cohorts-with-employers",
                PageHeading = "Cothorts with employers",
                PageHeading2 = $"You have <strong>{data.Count}</strong> cohorts that are that are with employers:"
            };
        }

        public async Task<CommitmentListViewModel> GetAllNewRequests(long providerId)
        {
            var data = (await GetAll(providerId, RequestStatus.NewRequest)).ToList();
            _logger.Info($"Provider getting all new request ({data.Count}) :{providerId}", providerId);

            return new CommitmentListViewModel
            {
                ProviderId = providerId,
                Commitments = await MapFrom(data, _latestMessageToProviderFunc),
                PageTitle = "New cohorts",
                PageId = "new-cohorts",
                PageHeading = "New cohorts",
                PageHeading2 = $"You have <strong>{data.ToList().Count}</strong> new cohorts:"
            };
        }

        public async Task<CommitmentListViewModel> GetAllReadyForReview(long providerId)
        {
            var data = (await GetAll(providerId, RequestStatus.ReadyForReview)).ToList();
            _logger.Info($"Provider getting all ready for review ({data.Count}) :{providerId}", providerId);
            return new CommitmentListViewModel
            {
                ProviderId = providerId,
                Commitments = await MapFrom(data, _latestMessageToProviderFunc),
                PageTitle = "Review cohorts",
                PageId = "review-cohorts-list",
                PageHeading = "Review cohorts",
                PageHeading2 = $"You have <strong>{data.Count}</strong> cohorts that are ready for review:"
            };
        }

        public async Task<CommitmentListViewModel> GetAllReadyForApproval(long providerId)
        {
            var data = (await GetAll(providerId, RequestStatus.ReadyForApproval)).ToList();
            _logger.Info($"Provider getting all ready for approval ({data.Count}) :{providerId}", providerId);
            
            return new CommitmentListViewModel
            {
                ProviderId = providerId,
                Commitments = await MapFrom(data, _latestMessageToProviderFunc),
                PageTitle = "Approve cohorts",
                PageId = "Approve cohorts",
                PageHeading =  "Approve cohorts",
                PageHeading2 =  $"You have <strong>{data.Count}</strong> cohorts that need your approval:"
            };
        }

        public async Task<IEnumerable<CommitmentListItem>> GetAll(long providerId, RequestStatus requestStatus)
        {
            _logger.Info($"Getting all commitments for provider:{providerId}", providerId);

            var data = await _mediator.SendAsync(new GetCommitmentsQueryRequest
            {
                ProviderId = providerId
            });

            return data.Commitments.Where(
                m => _statusCalculator.GetStatus(m.EditStatus, m.ApprenticeshipCount, m.LastAction, m.AgreementStatus)
                    == requestStatus);
        }

        public async Task<CommitmentDetailsViewModel> GetCommitmentDetails(long providerId, string hashedCommitmentId)
        {
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting commitment details:{commitmentId} for provider:{providerId}", providerId: providerId, commitmentId: commitmentId);

            var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                ProviderId = providerId,
                CommitmentId = commitmentId
            });

            AssertCommitmentStatus(data.Commitment, EditStatus.ProviderOnly);
            AssertCommitmentStatus(data.Commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed);

            var message = await GetLatestMessage(providerId, commitmentId, true);

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

        public async Task<CommitmentListItemViewModel> GetCommitmentCheckState(long providerId, string hashedCommitmentId)
        {
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);

            _logger.Info($"Getting commitment:{commitmentId} for provider:{providerId}", providerId, commitmentId);

            var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                ProviderId = providerId,
                CommitmentId = commitmentId
            });

            AssertCommitmentStatus(data.Commitment, EditStatus.ProviderOnly);
            AssertCommitmentStatus(data.Commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed);

            return MapFrom(data.Commitment);
        }

        public async Task<CommitmentListItemViewModel> GetCommitment(long providerId, string hashedCommitmentId)
        {
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);

            _logger.Info($"Getting commitment:{commitmentId} for provider:{providerId}", providerId, commitmentId);

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
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            await AssertCommitmentStatus(commitmentId, providerId);

            _logger.Info($"Getting apprenticeship:{apprenticeshipId} for provider:{providerId}", providerId: providerId, commitmentId: commitmentId, apprenticeshipId: apprenticeshipId);

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
            _logger.Info($"Getting info for creating apprenticeship for provider:{providerId} commitment:{commitmentId}", providerId: providerId, commitmentId: commitmentId);

            var apprenticeship = new ApprenticeshipViewModel
            {
                ProviderId = providerId,
                HashedCommitmentId = hashedCommitmentId
            };

            await AssertCommitmentStatus(commitmentId, providerId);

            return new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = apprenticeship,
                ApprenticeshipProgrammes = await GetTrainingProgrammes()
            };
        }

        public async Task CreateApprenticeship(ApprenticeshipViewModel apprenticeshipViewModel)
        {
            var apprenticeship = await MapFrom(apprenticeshipViewModel);
            await AssertCommitmentStatus(apprenticeship.CommitmentId, apprenticeship.ProviderId);

            await _mediator.SendAsync(new CreateApprenticeshipCommand
            {
                ProviderId = apprenticeshipViewModel.ProviderId,
                Apprenticeship = apprenticeship
            });

            _logger.Info($"Created apprenticeship for provider:{apprenticeshipViewModel.ProviderId} commitment:{apprenticeship.CommitmentId}", providerId: apprenticeship.ProviderId, commitmentId: apprenticeship.CommitmentId);
        }

        public async Task UpdateApprenticeship(ApprenticeshipViewModel apprenticeshipViewModel)
        {
            var apprenticeship = await MapFrom(apprenticeshipViewModel);
            await AssertCommitmentStatus(apprenticeship.CommitmentId, apprenticeship.ProviderId);

            await _mediator.SendAsync(new UpdateApprenticeshipCommand
            {
                ProviderId = apprenticeshipViewModel.ProviderId,
                Apprenticeship = apprenticeship
            });

            _logger.Info($"Updated apprenticeship for provider:{apprenticeshipViewModel.ProviderId} commitment:{apprenticeship.CommitmentId}", providerId: apprenticeship.ProviderId, commitmentId: apprenticeship.CommitmentId);
        }

        public async Task SubmitCommitment(long providerId, string hashedCommitmentId, SaveStatus saveStatus, string message)
        {
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            await AssertCommitmentStatus(commitmentId, providerId);

            _logger.Info($"Submitting ({saveStatus}) Commitment for provider:{providerId} commitment:{commitmentId}", providerId: providerId, commitmentId: commitmentId);

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
                _logger.Warn($"Commitment submited with illegal state, Save Status: {saveStatus}");
            }
        }

        public async Task<FinishEditingViewModel> GetFinishEditing(long providerId, string hashedCommitmentId)
        {
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);

            _logger.Info($"Get info for finish editing options for provider:{providerId} commitment:{commitmentId}", providerId: providerId, commitmentId: commitmentId);

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

        private async Task<string> GetLatestMessage(long? id, long commitmentId, bool isProvider)
        {
            if (id == null) return string.Empty;
            var allTasks = await _mediator.SendAsync(new GetTasksQueryRequest { Id = id.Value, IsProvider = isProvider });

            var taskForCommitment = allTasks?.Tasks
                .Select(x => new { Task = JsonConvert.DeserializeObject<CreateCommitmentTemplate>(x.Body), CreateDate = x.CreatedOn })
                .Where(x => x.Task != null && x.Task.CommitmentId == commitmentId)
                .OrderByDescending(x => x.CreateDate)
                .FirstOrDefault();

            var message = taskForCommitment?.Task?.Message ?? string.Empty;

            return message;
        }
        

        // TODO: Move mappers into own class
        private async Task<IEnumerable<CommitmentListItemViewModel>> MapFrom(List<CommitmentListItem> commitments, Func<CommitmentListItem, Task<string>> latestMessageFunc)
        {
            var commitmentsList = commitments.Select(m => MapFrom(m, latestMessageFunc)).ToList();

            return await Task.WhenAll(commitmentsList);
        }

        private async Task<CommitmentListItemViewModel> MapFrom(CommitmentListItem listItem, Func<CommitmentListItem, Task<string>> latestMessageFunc)
        {
            var message = await latestMessageFunc.Invoke(listItem);
            
            return new CommitmentListItemViewModel
            {
                HashedCommitmentId = _hashingService.HashValue(listItem.Id),
                Reference = listItem.Reference,
                LegalEntityName = listItem.LegalEntityName,
                ProviderName = listItem.ProviderName,
                Status = _statusCalculator.GetStatus(listItem.EditStatus, listItem.ApprenticeshipCount, listItem.LastAction, listItem.AgreementStatus),
                ShowViewLink = listItem.EditStatus == EditStatus.ProviderOnly,
                LatestMessage = message
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
                ProviderId = viewModel.ProviderId,
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
    }
}
