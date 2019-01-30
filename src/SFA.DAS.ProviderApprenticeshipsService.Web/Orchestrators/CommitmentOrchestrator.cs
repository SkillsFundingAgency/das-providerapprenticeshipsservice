using MediatR;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitments;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using CommitmentView = SFA.DAS.Commitments.Api.Types.Commitment.CommitmentView;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Application.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderHasRelationshipWithPermission;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class CommitmentOrchestrator : BaseCommitmentOrchestrator
    {
        private readonly IApprenticeshipCoreValidator _apprenticeshipCoreValidator;
        private readonly IApprenticeshipMapper _apprenticeshipMapper;
        private readonly ApprenticeshipViewModelUniqueUlnValidator _uniqueUlnValidator;
        private readonly ProviderApprenticeshipsServiceConfiguration _configuration;
        private readonly Func<int, string> _addSSuffix = i => i > 1 ? "s" : "";

        public CommitmentOrchestrator(IMediator mediator,
            IHashingService hashingService, IProviderCommitmentsLogger logger,
            ApprenticeshipViewModelUniqueUlnValidator uniqueUlnValidator,
            ProviderApprenticeshipsServiceConfiguration configuration,
            IApprenticeshipCoreValidator apprenticeshipCoreValidator,
            IApprenticeshipMapper apprenticeshipMapper)
            : base(mediator, hashingService, logger)
        {
            _uniqueUlnValidator = uniqueUlnValidator;
            _configuration = configuration;
            _apprenticeshipCoreValidator = apprenticeshipCoreValidator;
            _apprenticeshipMapper = apprenticeshipMapper;
        }

        public async Task<CohortsViewModel> GetCohorts(long providerId)
        {
            Logger.Info($"Getting cohorts :{providerId}", providerId);

            var relationshipResponse = await Mediator.Send(new GetProviderHasRelationshipWithPermissionQueryRequest
            {
                ProviderId = providerId,
                Permission = Operation.CreateCohort
            });
            var showDrafts = relationshipResponse.HasPermission;
            

            var data = await Mediator.Send(new GetCommitmentsQueryRequest
            {
                ProviderId = providerId
            });
            var commitmentStatus = data.Commitments.Select(m => m.GetStatus()).ToList();

            var model = new CohortsViewModel
            {
                DraftCount = commitmentStatus.Count(m =>
                    m == RequestStatus.NewRequest ),

                ReadyForReviewCount = commitmentStatus.Count(m =>
                m == RequestStatus.ReadyForReview
                || m == RequestStatus.ReadyForApproval),

                WithEmployerCount = commitmentStatus.Count(m => m == RequestStatus.SentForReview || m == RequestStatus.WithEmployerForApproval),
                TransferFundedCohortsCount = commitmentStatus.Count(m =>
                        m == RequestStatus.WithSenderForApproval
                        || m == RequestStatus.RejectedBySender),

                HasSignedTheAgreement = await IsSignedAgreement(providerId) == ProviderAgreementStatus.Agreed,
                SignAgreementUrl = _configuration.ContractAgreementsUrl,
                ShowDrafts = showDrafts
            };

            return model;
        }

        public async Task<CommitmentListViewModel> GetAllReadyForReview(long providerId)
        {
            var data = (await GetAllCommitmentsWithTheseStatuses(providerId,
                RequestStatus.ReadyForReview, RequestStatus.ReadyForApproval)).ToList();

            Logger.Info($"Provider getting all new, ReadyForReview or ReadyForApproval ({data.Count}) :{providerId}", providerId);

            return new CommitmentListViewModel
            {
                ProviderId = providerId,
                Commitments = MapFrom(data, true),
                PageTitle = "Cohorts to review, update or approve",
                PageId = "review-cohorts-list",
                PageHeading = "Cohorts to review, update or approve",
                PageHeading2 = $"You have <strong>{data.Count}</strong> cohort{_addSSuffix(data.Count)} ready for you to review, update or approve:",
                HasSignedAgreement = await IsSignedAgreement(providerId) == ProviderAgreementStatus.Agreed
            };
        }

        public async Task<CommitmentListViewModel> GetAllWithEmployer(long providerId)
        {
            var data = (await GetAllCommitmentsWithTheseStatuses(providerId,
                RequestStatus.SentForReview, RequestStatus.WithEmployerForApproval)).ToList();
            Logger.Info($"Provider getting all with employer ({data.Count}) :{providerId}", providerId);

            return new CommitmentListViewModel
            {
                ProviderId = providerId,
                Commitments = MapFrom(data, false),
                PageTitle = "Cohorts with employers",
                PageId = "cohorts-with-employers",
                PageHeading = "Cohorts with employers",
                PageHeading2 = $"You have <strong>{data.Count}</strong> cohort{_addSSuffix(data.Count)} with an employer for them to add details, review or approve:",
                HasSignedAgreement = await IsSignedAgreement(providerId) == ProviderAgreementStatus.Agreed
            };
        }

        public async Task<TransferFundedViewModel> GetAllTransferFunded(long providerId)
        {
            var data = (await GetAllCommitmentsWithTheseStatuses(providerId, RequestStatus.WithSenderForApproval, RequestStatus.RejectedBySender)).ToList();
            Logger.Info($"Provider getting all transfer funded ({data.Count}) :{providerId}", providerId);

            return new TransferFundedViewModel
            {
                ProviderId = providerId,
                Commitments = data.Select(MapToTransferFundedListItemViewModel)
            };
        }

        public async Task<CommitmentListViewModel> GetAllDrafts(long providerId)
        {
            var data = (await GetAllCommitmentsWithTheseStatuses(providerId,
                RequestStatus.NewRequest)).ToList();

            Logger.Info($"Provider getting all New ({data.Count}) :{providerId}", providerId);

            return new CommitmentListViewModel
            {
                ProviderId = providerId,
                Commitments = MapFrom(data, true),
                PageTitle = "Draft cohorts",
                PageId = "draft-cohorts-list",
                PageHeading = "Draft cohorts",
                PageHeading2 = $"You have <strong>{data.Count}</strong> cohort{_addSSuffix(data.Count)} waiting to be sent to an employer:",
                HasSignedAgreement = await IsSignedAgreement(providerId) == ProviderAgreementStatus.Agreed
            };
        }

        public async Task<DeleteConfirmationViewModel> GetDeleteConfirmationModel(long providerId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);

            var apprenticeship = await Mediator.Send(new GetApprenticeshipQueryRequest
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId
            });

            return new DeleteConfirmationViewModel
            {
                ProviderId = providerId,
                HashedCommitmentId = hashedCommitmentId,
                HashedApprenticeshipId = hashedApprenticeshipId,
                ApprenticeshipName = apprenticeship.Apprenticeship.ApprenticeshipName,
                DateOfBirth = apprenticeship.Apprenticeship.DateOfBirth.HasValue ? apprenticeship.Apprenticeship.DateOfBirth.Value.ToGdsFormat() : string.Empty
            };
        }

        public async Task DeleteCommitment(string userId, long providerId, string hashedCommitmentId, SignInUserModel signinUser)
        {
            var commitmentId = HashingService.DecodeValue(hashedCommitmentId);
            Logger.Info($"Deleting commitment {hashedCommitmentId}", providerId, commitmentId);

            await Mediator.Send(new DeleteCommitmentCommand
            {
                UserId = userId,
                ProviderId = providerId,
                CommitmentId = commitmentId,
                UserDisplayName = signinUser.DisplayName,
                UserEmailAddress = signinUser.Email
            });
        }

        public async Task<string> DeleteApprenticeship(string userId, DeleteConfirmationViewModel viewModel, SignInUserModel signinUser)
        {
            var apprenticeshipId = HashingService.DecodeValue(viewModel.HashedApprenticeshipId);
            Logger.Info($"Deleting apprenticeship {apprenticeshipId}", providerId: viewModel.ProviderId, apprenticeshipId: apprenticeshipId);

            var apprenticeship = await Mediator.Send(new GetApprenticeshipQueryRequest
            {
                ProviderId = viewModel.ProviderId,
                ApprenticeshipId = apprenticeshipId
            });

            await Mediator.Send(new DeleteApprenticeshipCommand
            {
                UserId = userId,
                ProviderId = viewModel.ProviderId,
                ApprenticeshipId = apprenticeshipId,
                UserDisplayName = signinUser.DisplayName,
                UserEmailAddress = signinUser.Email
            });

            return apprenticeship.Apprenticeship.ApprenticeshipName;
        }

        public async Task<AgreementNotSignedViewModel> GetAgreementPage(long providerId, string hashedCommitmentId)
        {
            var model = new AgreementNotSignedViewModel
            {
                ProviderId = providerId,
                HashedCommitmentId = hashedCommitmentId,
                ReviewAgreementUrl = _configuration.ContractAgreementsUrl,
                IsSignedAgreement = await IsSignedAgreement(providerId) == ProviderAgreementStatus.Agreed
            };
            return model;
        }

        public async Task<IEnumerable<CommitmentListItem>> GetAllCommitmentsWithTheseStatuses(long providerId, params RequestStatus[] requestStatus)
        {
            Logger.Info($"Getting all commitments for provider:{providerId}", providerId);

            var data = await Mediator.Send(new GetCommitmentsQueryRequest
            {
                ProviderId = providerId
            });

            return data.Commitments.Where(m => requestStatus.Contains(m.GetStatus()));
        }

        public async Task<ProviderAgreementStatus> IsSignedAgreement(long providerId)
        {
            var data = await Mediator.Send(
                new GetProviderAgreementQueryRequest
                {
                    ProviderId = providerId
                });

            return data.HasAgreement;
        }

        public async Task<CommitmentDetailsViewModel> GetCommitmentDetails(long providerId, string hashedCommitmentId)
        {
            var commitmentId = HashingService.DecodeValue(hashedCommitmentId);

            var commitment = await GetCommitment(providerId, commitmentId);

            AssertCommitmentStatus(commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed, AgreementStatus.BothAgreed);

            var overlapping = await Mediator.Send(
                new GetOverlappingApprenticeshipsQueryRequest
                {
                    Apprenticeship = commitment.Apprenticeships
                });

            var apprenticeships = MapFrom(commitment.Apprenticeships, overlapping);
            var trainingProgrammes = await GetTrainingProgrammes(!commitment.IsTransfer());

            var errors = new Dictionary<string, string>();
            var warnings = new Dictionary<string, string>();

            var apprenticeshipGroups = apprenticeships.OrderBy(x => x.TrainingName).GroupBy(x => x.TrainingCode)
                .Select(g => new ApprenticeshipListItemGroupViewModel(g.OrderBy(x => x.CanBeApproved).ToList(), trainingProgrammes.FirstOrDefault(x => x.Id == g.Key)))
                    .ToList();

            foreach (var apprenticeshipGroup in apprenticeshipGroups)
            {
                if (apprenticeshipGroup.OverlapErrorCount > 0)
                {
                    var trainingTitle = string.IsNullOrEmpty(apprenticeshipGroup.TrainingProgramme?.Title)
                        ? string.Empty
                        : $":{apprenticeshipGroup.TrainingProgramme.Title}";
                    errors.Add(apprenticeshipGroup.GroupId, $"Overlapping training dates{trainingTitle}");
                }

                if (apprenticeshipGroup.ApprenticeshipsOverFundingLimit > 0) // for this to be true, there must be a TrainingProgramme, so no need to cater for null
                    warnings.Add(apprenticeshipGroup.GroupId, $"Cost for {apprenticeshipGroup.TrainingProgramme.Title}");
            }

            return new CommitmentDetailsViewModel
            {
                ProviderId = providerId,
                HashedCommitmentId = hashedCommitmentId,
                LegalEntityName = commitment.LegalEntityName,
                Reference = commitment.Reference,
                Status = commitment.GetStatus(),
                HasApprenticeships = apprenticeships.Count > 0,
                Apprenticeships = apprenticeships,
                LatestMessage = GetLatestMessage(commitment.Messages, true)?.Message,
                PendingChanges = commitment.AgreementStatus != AgreementStatus.EmployerAgreed,
                ApprenticeshipGroups = apprenticeshipGroups,
                IsReadOnly = commitment.EditStatus != EditStatus.ProviderOnly,
                IsFundedByTransfer = commitment.IsTransfer(),
                Errors = errors,
                Warnings = warnings
            };
        }

        public async Task<DeleteCommitmentViewModel> GetDeleteCommitmentModel(long providerId, string hashedCommitmentId)
        {
            var commitment = await GetCommitment(providerId, hashedCommitmentId);

            string TextOrDefault(string txt) => !string.IsNullOrEmpty(txt) ? txt : "without training course details";

            var programmeSummary = commitment.Apprenticeships
                .GroupBy(m => m.TrainingName)
                .Select(m => $"{m.Count()} {TextOrDefault(m.Key)}")
                .ToList();

            return new DeleteCommitmentViewModel
            {
                ProviderId = providerId,
                HashedCommitmentId = hashedCommitmentId,
                LegalEntityName = commitment.LegalEntityName,
                CohortReference = hashedCommitmentId,
                NumberOfApprenticeships = commitment.Apprenticeships.Count,
                ApprenticeshipTrainingProgrammes = programmeSummary
            };
        }

        public async Task<CommitmentListItemViewModel> GetCommitmentCheckState(long providerId, string hashedCommitmentId)
        {
            var commitment = await GetCommitment(providerId, hashedCommitmentId);

            AssertCommitmentStatus(commitment, EditStatus.ProviderOnly);
            AssertCommitmentStatus(commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed);

            return MapFrom(commitment, GetLatestMessage(commitment.Messages, true)?.Message);
        }

        public async Task<ExtendedApprenticeshipViewModel> GetApprenticeship(long providerId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);
            var commitmentId = HashingService.DecodeValue(hashedCommitmentId);

            Logger.Info($"Getting apprenticeship:{apprenticeshipId} for provider:{providerId}", providerId: providerId, commitmentId: commitmentId, apprenticeshipId: apprenticeshipId);

            var data = await Mediator.Send(new GetApprenticeshipQueryRequest
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId
            });

            var commitment = await GetCommitment(providerId, commitmentId);
            AssertCommitmentStatus(commitment);

            var overlappingErrors = await Mediator.Send(
                new GetOverlappingApprenticeshipsQueryRequest
                {
                    Apprenticeship = new List<Apprenticeship> { data.Apprenticeship }
                });

            var apprenticeship = _apprenticeshipMapper.MapApprenticeship(data.Apprenticeship, commitment);

            apprenticeship.ProviderId = providerId;

            return new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = apprenticeship,
                ApprenticeshipProgrammes = await GetTrainingProgrammes(!commitment.IsTransfer()),
                ValidationErrors = _apprenticeshipCoreValidator.MapOverlappingErrors(overlappingErrors)
            };
        }

        public async Task<ApprenticeshipViewModel> GetApprenticeshipViewModel(long providerId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var apprenticeshipId = HashingService.DecodeValue(hashedApprenticeshipId);
            var commitmentId = HashingService.DecodeValue(hashedCommitmentId);

            Logger.Info($"Getting apprenticeship:{apprenticeshipId} for provider:{providerId}", providerId: providerId, commitmentId: commitmentId, apprenticeshipId: apprenticeshipId);

            var data = await Mediator.Send(new GetApprenticeshipQueryRequest
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId
            });

            var commitment = await GetCommitment(providerId, commitmentId);

            var apprenticeship = _apprenticeshipMapper.MapApprenticeship(data.Apprenticeship, commitment);

            apprenticeship.ProviderId = providerId;

            return apprenticeship;
        }

        public async Task<ExtendedApprenticeshipViewModel> GetCreateApprenticeshipViewModel(long providerId, string hashedCommitmentId)
        {
            Logger.Info("Getting info for creating apprenticeship");

            var commitment = await GetCommitment(providerId, hashedCommitmentId);
            AssertCommitmentStatus(commitment);

            var apprenticeship = new ApprenticeshipViewModel
            {
                ProviderId = providerId,
                HashedCommitmentId = hashedCommitmentId,
                IsPaidForByTransfer = commitment.IsTransfer()
            };

            return new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = apprenticeship,
                ApprenticeshipProgrammes = await GetTrainingProgrammes(!commitment.IsTransfer())
            };
        }

        public async Task CreateApprenticeship(string userId, ApprenticeshipViewModel apprenticeshipViewModel, SignInUserModel signInUser)
        {
            var apprenticeship = await _apprenticeshipMapper.MapApprenticeship(apprenticeshipViewModel);

            await AssertCommitmentStatus(apprenticeship.CommitmentId, apprenticeship.ProviderId);

            await Mediator.Send(new CreateApprenticeshipCommand
            {
                UserId = userId,
                ProviderId = apprenticeshipViewModel.ProviderId,
                Apprenticeship = apprenticeship,
                UserEmailAddress = signInUser.Email,
                UserDisplayName = signInUser.DisplayName
            });

            Logger.Info($"Created apprenticeship for provider:{apprenticeshipViewModel.ProviderId} commitment:{apprenticeship.CommitmentId}", providerId: apprenticeship.ProviderId, commitmentId: apprenticeship.CommitmentId);
        }

        public async Task UpdateApprenticeship(string userId, ApprenticeshipViewModel apprenticeshipViewModel, SignInUserModel currentUser)
        {
            var apprenticeship = await _apprenticeshipMapper.MapApprenticeship(apprenticeshipViewModel);
            await AssertCommitmentStatus(apprenticeship.CommitmentId, apprenticeship.ProviderId);

            await Mediator.Send(new UpdateApprenticeshipCommand
            {
                UserId = userId,
                ProviderId = apprenticeshipViewModel.ProviderId,
                Apprenticeship = apprenticeship,
                UserEmailAddress = currentUser.Email,
                UserDisplayName = currentUser.DisplayName
            });

            Logger.Info($"Updated apprenticeship for provider:{apprenticeshipViewModel.ProviderId} commitment:{apprenticeship.CommitmentId}", providerId: apprenticeship.ProviderId, commitmentId: apprenticeship.CommitmentId);
        }

        public async Task<bool> AnyCohortsForStatus(long providerId, RequestStatus requestStatus)
        {
            var data = await GetAllCommitmentsWithTheseStatuses(providerId, requestStatus);
            return data.Any();
        }

        public async Task SubmitCommitment(string currentUserId, long providerId, string hashedCommitmentId, SaveStatus saveStatus, string message, SignInUserModel currentUser)
        {
            var commitmentId = HashingService.DecodeValue(hashedCommitmentId);
            await AssertCommitmentStatus(commitmentId, providerId);

            Logger.Info($"Submitting ({saveStatus}) Commitment for provider:{providerId} commitment:{commitmentId}", providerId: providerId, commitmentId: commitmentId);

            if (saveStatus == SaveStatus.Approve || saveStatus == SaveStatus.ApproveAndSend)
            {
                var isSigned = await IsSignedAgreement(providerId) == ProviderAgreementStatus.Agreed;
                if (!isSigned)
                    throw new InvalidStateException("Cannot approve commitment when no agreement signed");
            }

            LastAction lastAction;
            switch (saveStatus)
            {
                case SaveStatus.AmendAndSend:
                    lastAction = LastAction.Amend;
                    break;
                case SaveStatus.Approve:
                    lastAction = LastAction.Approve;
                    break;
                case SaveStatus.ApproveAndSend:
                    lastAction = LastAction.Approve;
                    break;
                case SaveStatus.Save:
                    lastAction = LastAction.None;
                    break;
                default:
                    lastAction = LastAction.None;
                    break;
            }

            await
                Mediator.Send(
                    new SubmitCommitmentCommand
                    {
                        ProviderId = providerId,
                        HashedCommitmentId = hashedCommitmentId,
                        CommitmentId = commitmentId,
                        Message = message,
                        LastAction = lastAction,
                        CreateTask = (saveStatus == SaveStatus.ApproveAndSend || saveStatus == SaveStatus.AmendAndSend),
                        UserDisplayName = currentUser.DisplayName,
                        UserEmailAddress = currentUser.Email,
                        UserId = currentUserId
                    });
        }

        public async Task<FinishEditingViewModel> GetFinishEditing(long providerId, string hashedCommitmentId)
        {
            var commitmentId = HashingService.DecodeValue(hashedCommitmentId);

            Logger.Info($"Get info for finish editing options for provider:{providerId} commitment:{commitmentId}", providerId: providerId, commitmentId: commitmentId);

            var commitment = await GetCommitment(providerId, commitmentId);

            AssertCommitmentStatus(commitment, EditStatus.ProviderOnly);
            AssertCommitmentStatus(commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed);

            var overlaps = await Mediator.Send(
                new GetOverlappingApprenticeshipsQueryRequest
                {
                    Apprenticeship = commitment.Apprenticeships
                });

            return new FinishEditingViewModel
            {
                HashedCommitmentId = hashedCommitmentId,
                ProviderId = providerId,
                ReadyForApproval = commitment.CanBeApproved,
                ApprovalState = GetApprovalState(commitment),
                HasApprenticeships = commitment.Apprenticeships.Any(),
                InvalidApprenticeshipCount = commitment.Apprenticeships.Count(x => !x.CanBeApproved),
                HasSignedTheAgreement = await IsSignedAgreement(providerId) == ProviderAgreementStatus.Agreed,
                SignAgreementUrl = _configuration.ContractAgreementsUrl,
                HasOverlappingErrors = overlaps.Overlaps.Any()
            };
        }

        private static ApprovalState GetApprovalState(CommitmentView commitment)
        {
            if (!commitment.Apprenticeships.Any()) return ApprovalState.ApproveAndSend;

            var approvalState = commitment.Apprenticeships.Any(m => m.AgreementStatus == AgreementStatus.NotAgreed
                                   || m.AgreementStatus == AgreementStatus.ProviderAgreed) ? ApprovalState.ApproveAndSend : ApprovalState.ApproveOnly;

            return approvalState;
        }

        private IList<ApprenticeshipListItemViewModel> MapFrom(IEnumerable<Apprenticeship> apprenticeships, GetOverlappingApprenticeshipsQueryResponse overlaps)
        {
            var apprenticeViewModels = apprenticeships
                .Select(x => new ApprenticeshipListItemViewModel
                {
                    HashedApprenticeshipId = HashingService.HashValue(x.Id),
                    ApprenticeshipName = x.ApprenticeshipName,
                    ApprenticeDateOfBirth = x.DateOfBirth,
                    ULN = x.ULN,
                    TrainingCode = x.TrainingCode,
                    TrainingName = x.TrainingName,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Cost = x.Cost,
                    CanBeApproved = x.CanBeApproved,
                    OverlappingApprenticeships = overlaps?.GetOverlappingApprenticeships(x.Id)
                }).ToList();

            return apprenticeViewModels;
        }

        // TODO: Move mappers into own class
        private IEnumerable<CommitmentListItemViewModel> MapFrom(IEnumerable<CommitmentListItem> commitments, bool showProviderMessage)
        {
            return commitments.Select(m => MapFrom(m, GetLatestMessage(m.Messages, showProviderMessage)?.Message));
        }

        private MessageView GetLatestMessage(IEnumerable<MessageView> messages, bool showProviderMessage)
        {
            return messages.Where(x => x.CreatedBy == (showProviderMessage ? MessageCreator.Employer : MessageCreator.Provider)).OrderByDescending(x => x.CreatedDateTime).FirstOrDefault();
        }

        private CommitmentListItemViewModel MapFrom(CommitmentListItem listItem, string lastestMessage)
        {
            return new CommitmentListItemViewModel
            {
                HashedCommitmentId = HashingService.HashValue(listItem.Id),
                Reference = listItem.Reference,
                LegalEntityName = listItem.LegalEntityName,
                ProviderName = listItem.ProviderName,
                Status = listItem.GetStatus(),
                LatestMessage = lastestMessage
            };
        }

        private CommitmentListItemViewModel MapFrom(CommitmentView listItem, string latestMessage)
        {
            return new CommitmentListItemViewModel
            {
                HashedCommitmentId = HashingService.HashValue(listItem.Id),
                Reference = listItem.Reference,
                LegalEntityName = listItem.LegalEntityName,
                ProviderName = listItem.ProviderName,
                Status = listItem.GetStatus(),
                EmployerAccountId = listItem.EmployerAccountId,
                LatestMessage = latestMessage
            };
        }

        private TransferFundedListItemViewModel MapToTransferFundedListItemViewModel(CommitmentListItem listItem)
        {
            return new TransferFundedListItemViewModel
            {
                HashedCommitmentId = HashingService.HashValue(listItem.Id),
                ReceivingEmployerName = listItem.LegalEntityName,
                Status = listItem.TransferApprovalStatus
            };
        }

        public async Task<AcknowledgementViewModel> GetAcknowledgementViewModel(long providerId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            var commitment = await GetCommitment(providerId, hashedCommitmentId);
            var result = new AcknowledgementViewModel
            {
                CommitmentReference = commitment.Reference,
                EmployerName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName,
                Message = GetLatestMessage(commitment.Messages, false)?.Message,
                RedirectUrl = string.Empty,
                RedirectLinkText = string.Empty,
                PageTitle = saveStatus == SaveStatus.ApproveAndSend
                    ? "Cohort approved and sent to employer"
                    : "Cohort sent for review",
                WhatHappensNext = new List<string>()
            };

            switch (saveStatus)
            {
                case SaveStatus.ApproveAndSend when commitment.IsTransfer():
                    result.WhatHappensNext.AddRange(new[]
                    {
                        "The employer will receive your cohort and will either confirm the information is correct or contact you to suggest changes.",
                        "Once the employer approves the cohort, a transfer request will be sent to the funding employer to review.",
                        "You will receive a notification once the funding employer approves or rejects the transfer request. You can view the progress of a request from the 'With transfer sending employers' status screen."
                    });
                    break;
                case SaveStatus.ApproveAndSend:
                    result.WhatHappensNext.Add("The employer will review the cohort and either approve it or contact you with an update.");
                    break;
                default:
                    result.WhatHappensNext.Add("The updated cohort will appear in the employer’s account for them to review.");
                    break;
            }
            return result;
        }

        public async Task<ApprovedViewModel> GetApprovedViewModel(long providerId, string hashedCommitmentId)
        {
            var commitment = await GetCommitment(providerId, hashedCommitmentId);

            return new ApprovedViewModel
            {
                Headline = "Cohort approved",
                CommitmentReference = commitment.Reference,
                EmployerName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName,
                IsTransfer = commitment.IsTransfer(),
                HasOtherCohortsAwaitingApproval = await AnyCohortsForStatus(providerId, RequestStatus.ReadyForApproval)
            };
        }

        public async Task<Dictionary<string, string>> ValidateApprenticeship(ApprenticeshipViewModel viewModel)
        {
            var overlappingErrors = await Mediator.Send(
                new GetOverlappingApprenticeshipsQueryRequest
                {
                    Apprenticeship = new List<Apprenticeship> { await _apprenticeshipMapper.MapApprenticeship(viewModel) }
                });

            var result = _apprenticeshipCoreValidator.MapOverlappingErrors(overlappingErrors);

            // EndDate is optional before approval
            if (viewModel.EndDate.DateTime != null)
            {
                var endDateError = _apprenticeshipCoreValidator.CheckEndDateInFuture(viewModel.EndDate);
                if (endDateError != null)
                    result.AddIfNotExists(endDateError.Value);
            }

            var uniqueUlnValidationResult = await _uniqueUlnValidator.ValidateAsyncOverride(viewModel);
            if (!uniqueUlnValidationResult.IsValid)
            {
                foreach (var error in uniqueUlnValidationResult.Errors)
                {
                    result.AddIfNotExists(error.PropertyName, error.ErrorMessage);
                }
            }

            return result;
        }
    }
}
