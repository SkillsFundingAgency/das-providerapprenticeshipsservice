using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.ReviewApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.TriageApprenticeshipDataLocks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UndoApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateDataLock;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.ApprenticeshipSearch;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLockSummary;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipPriceHistory;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetPendingApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.ApprovedApprenticeshipValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public sealed class ManageApprenticesOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IProviderCommitmentsLogger _logger;
        private readonly IHashingService _hashingService;
        private readonly IApprenticeshipMapper _apprenticeshipMapper;
        private readonly IApprovedApprenticeshipValidator _approvedApprenticeshipValidator;
        private readonly IApprenticeshipFiltersMapper _apprenticeshipFiltersMapper;
        private readonly IDataLockMapper _dataLockMapper;
        private string _searchPlaceholderText;

        public ManageApprenticesOrchestrator(IMediator mediator,
            IHashingService hashingService,
            IProviderCommitmentsLogger logger,
            IApprenticeshipMapper apprenticeshipMapper,
            IApprovedApprenticeshipValidator approvedApprenticeshipValidator,
            IApprenticeshipFiltersMapper apprenticeshipFiltersMapper,
            IDataLockMapper dataLockMapper)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (apprenticeshipMapper == null)
                throw new ArgumentNullException(nameof(apprenticeshipMapper));
            if(approvedApprenticeshipValidator == null)
                throw new ArgumentNullException(nameof(approvedApprenticeshipValidator));
            if(apprenticeshipFiltersMapper == null)
                throw new ArgumentNullException(nameof(IApprenticeshipFiltersMapper));
            if(dataLockMapper == null)
                throw new ArgumentNullException(nameof(IDataLockMapper));

            _mediator = mediator;
            _hashingService = hashingService;
            _logger = logger;
            _apprenticeshipMapper = apprenticeshipMapper;
            _approvedApprenticeshipValidator = approvedApprenticeshipValidator;
            _apprenticeshipFiltersMapper = apprenticeshipFiltersMapper;
            _dataLockMapper = dataLockMapper;
            _searchPlaceholderText = "Enter a name or ULN";
        }

        public async Task<ManageApprenticeshipsViewModel> GetApprenticeships(long providerId, ApprenticeshipFiltersViewModel filters)
        {
            _logger.Info($"Getting On-programme apprenticeships for provider: {providerId}", providerId: providerId);

            if (filters.SearchInput?.Trim() == _searchPlaceholderText.Trim())
                filters.SearchInput = string.Empty;

            var searchQuery = _apprenticeshipFiltersMapper.MapToApprenticeshipSearchQuery(filters);

            var searchResponse = await _mediator.SendAsync(new ApprenticeshipSearchQueryRequest
            {
                ProviderId = providerId,
                Query = searchQuery
            });

            var apprenticeships = searchResponse.Apprenticeships
                .OrderBy(m => m.ApprenticeshipName)
                .Select(m => _apprenticeshipMapper.MapApprenticeshipDetails(m))
                .ToList();

            var filterOptions = _apprenticeshipFiltersMapper.Map(searchResponse.Facets);
            filterOptions.SearchInput = searchResponse.SearchKeyword;
            filterOptions.ResetFilter = false;

            return new ManageApprenticeshipsViewModel
            {
                ProviderId = providerId,
                Apprenticeships = apprenticeships,
                Filters = filterOptions,
                TotalResults = searchResponse.TotalApprenticeships,
                TotalApprenticeshipsBeforeFilter = searchResponse.TotalApprenticeshipsBeforeFilter,
                PageNumber = searchResponse.PageNumber,
                TotalPages = searchResponse.TotalPages,
                PageSize = searchResponse.PageSize,
                SearchInputPlaceholder = _searchPlaceholderText
            };
        }

        public async Task<ApprenticeshipDetailsViewModel> GetApprenticeshipViewModel(long providerId, string hashedApprenticeshipId)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info($"Getting On-programme apprenticeships Provider: {providerId}, ApprenticeshipId: {apprenticeshipId}", providerId: providerId, apprenticeshipId: apprenticeshipId);

            var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest { ProviderId = providerId, ApprenticeshipId = apprenticeshipId });

            var dataLockSummary = await _mediator.SendAsync(new GetApprenticeshipDataLockSummaryQueryRequest
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId
            });

            var result = _apprenticeshipMapper.MapApprenticeshipDetails(data.Apprenticeship);

            result.DataLockSummaryViewModel = await _dataLockMapper.MapDataLockSummary(dataLockSummary.DataLockSummary);

            return result;

        }

        public async Task<Apprenticeship> GetApprenticeship(long providerId, string hashedApprenticeshipId)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info($"Getting On-programme apprenticeships Provider: {providerId}, ApprenticeshipId: {apprenticeshipId}", providerId: providerId, apprenticeshipId: apprenticeshipId);

            var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest { ProviderId = providerId, ApprenticeshipId = apprenticeshipId });

            return data.Apprenticeship;
        }

        public async Task<ExtendedApprenticeshipViewModel> GetApprenticeshipForEdit(long providerId, string hashedApprenticeshipId)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);
            await AssertNoPendingApprenticeshipUpdate(providerId, apprenticeshipId);

            var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId
            });

            AssertApprenticeshipIsEditable(data.Apprenticeship);

            var overlappingErrors = await _mediator.SendAsync(
                new GetOverlappingApprenticeshipsQueryRequest
                {
                    Apprenticeship = new List<Apprenticeship> { data.Apprenticeship }
                });

            var apprenticeship = _apprenticeshipMapper.MapApprenticeship(data.Apprenticeship);

            return new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = apprenticeship,
                ApprenticeshipProgrammes = await GetTrainingProgrammes(),
                ValidationErrors = _apprenticeshipMapper.MapOverlappingErrors(overlappingErrors)
            };
        }

        public async Task<Dictionary<string, string>> ValidateEditApprenticeship(ApprenticeshipViewModel model, CreateApprenticeshipUpdateViewModel updateViewModel)
        {
            var result = new Dictionary<string, string>();

            var overlappingErrors = await _mediator.SendAsync(
                new GetOverlappingApprenticeshipsQueryRequest
                {
                    Apprenticeship = new List<Apprenticeship> { await _apprenticeshipMapper.MapApprenticeship(model) }
                });

            foreach (var overlap in _apprenticeshipMapper.MapOverlappingErrors(overlappingErrors))
            {
                result.Add(overlap.Key, overlap.Value);
            }

            foreach (var error in _approvedApprenticeshipValidator.Validate(model))
            {
                result.Add(error.Key, error.Value);
            }

            foreach (var error in _approvedApprenticeshipValidator.ValidateAcademicYear(updateViewModel))
            {
                result.AddIfNotExists(error.Key, error.Value);
            }

            return result;
        }

        public async Task<CreateApprenticeshipUpdateViewModel> GetConfirmChangesModel(long providerId, string hashedApprenticeshipId, ApprenticeshipViewModel apprenticeship)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);
            await AssertNoPendingApprenticeshipUpdate(providerId, apprenticeshipId);

            var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId
            });

            var viewModel = await _apprenticeshipMapper.CompareAndMapToCreateUpdateApprenticeshipViewModel(data.Apprenticeship, apprenticeship);
            return viewModel;
        }

        public async Task CreateApprenticeshipUpdate(CreateApprenticeshipUpdateViewModel updateApprenticeship, long providerId, string userId, SignInUserModel signedInUser)
        {
            await _mediator.SendAsync(new CreateApprenticeshipUpdateCommand
            {
                ProviderId = providerId,
                ApprenticeshipUpdate = _apprenticeshipMapper.MapApprenticeshipUpdate(updateApprenticeship),
                UserId = userId,
                UserDisplayName = signedInUser.DisplayName,
                UserEmailAddress = signedInUser.Email
            });
        }

        public async Task<ReviewApprenticeshipUpdateViewModel> GetReviewApprenticeshipUpdateModel(long providerId, string hashedApprenticeshipId)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            var update = await _mediator.SendAsync(new GetPendingApprenticeshipUpdateQueryRequest
            {
                ApprenticeshipId = apprenticeshipId,
                ProviderId = providerId
            });

            AssertApprenticeshipUpdateReviewable(update.ApprenticeshipUpdate);

            var original = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId
            });

            var viewModel = _apprenticeshipMapper.MapApprenticeshipUpdateViewModel<ReviewApprenticeshipUpdateViewModel>(original.Apprenticeship, update.ApprenticeshipUpdate);
            return viewModel;
        }

        public async Task SubmitReviewApprenticeshipUpdate(long providerId, string hashedApprenticeshipId, string userId, bool isApproved, SignInUserModel signedInUser)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            await _mediator.SendAsync(new ReviewApprenticeshipUpdateCommand
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId,
                UserId = userId,
                IsApproved = isApproved,
                UserDisplayName = signedInUser.DisplayName,
                UserEmailAddress = signedInUser.Email
            });
        }

        public async Task<UndoApprenticeshipUpdateViewModel> GetUndoApprenticeshipUpdateModel(long providerId, string hashedApprenticeshipId)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            var update = await _mediator.SendAsync(new GetPendingApprenticeshipUpdateQueryRequest
            {
                ApprenticeshipId = apprenticeshipId,
                ProviderId = providerId
            });

            AssertApprenticeshipUpdateUndoable(update.ApprenticeshipUpdate);

            var original = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId
            });

            var viewModel = _apprenticeshipMapper.MapApprenticeshipUpdateViewModel<UndoApprenticeshipUpdateViewModel>(original.Apprenticeship, update.ApprenticeshipUpdate);
            return viewModel;
        }

        public async Task SubmitUndoApprenticeshipUpdate(long providerId, string hashedApprenticeshipId, string userId, SignInUserModel signedInUser)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            await _mediator.SendAsync(new UndoApprenticeshipUpdateCommand
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId,
                UserId = userId,
                UserDisplayName = signedInUser.DisplayName,
                UserEmailAddress = signedInUser.Email
            });
        }

        public async Task<DataLockMismatchViewModel> GetApprenticeshipMismatchDataLock(long providerId, string hashedApprenticeshipId)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info($"Getting apprenticeship datalock for provider: {providerId}", providerId: providerId, apprenticeshipId: apprenticeshipId);

            var datalockSummary = await _mediator.SendAsync(new GetApprenticeshipDataLockSummaryQueryRequest
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId
            });

            var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId
            });

            var priceHistory = await _mediator.SendAsync(new GetApprenticeshipPriceHistoryQueryRequest
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId
            });

            var datalockSummaryViewModel =
                await _dataLockMapper.MapDataLockSummary(datalockSummary.DataLockSummary);

            var dasRecordViewModel = _apprenticeshipMapper.MapApprenticeship(data.Apprenticeship);

            return new DataLockMismatchViewModel
                       {
                            ProviderId = providerId,
                            HashedApprenticeshipId = hashedApprenticeshipId,
                            DasApprenticeship = dasRecordViewModel,
                            DataLockSummaryViewModel = datalockSummaryViewModel,
                            EmployerName = data.Apprenticeship.LegalEntityName,
                            PriceHistory = _apprenticeshipMapper.MapPriceHistory(priceHistory.History)
            };
        }

        public async Task<ConfirmRestartViewModel> GetConfirmRestartViewModel(long providerId, string hashedApprenticeshipId)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info($"Getting apprenticeship restart request for provider: {providerId}, apprenticeship: {apprenticeshipId}", providerId, apprenticeshipId);

            var dataLock = await GetApprenticeshipMismatchDataLock(providerId, hashedApprenticeshipId);
            return new ConfirmRestartViewModel
            {
                ProviderId = providerId,
                HashedApprenticeshipId = hashedApprenticeshipId,
                DataMismatchModel = dataLock,
                DataLockEventId = dataLock.DataLockSummaryViewModel.DataLockWithCourseMismatch
                    .OrderBy(x => x.IlrEffectiveFromDate)
                    .First(x => x.TriageStatusViewModel == TriageStatusViewModel.Unknown)
                    .DataLockEventId
            };
        }

        public async Task RequestRestart(long providerId, long dataLockEventId, string hashedApprenticeshipId, string userId)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            await _mediator.SendAsync(new UpdateDataLockCommand
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId,
                DataLockEventId = dataLockEventId,
                TriageStatus = TriageStatus.Restart,
                UserId = userId
            });
        }

        public async Task UpdateDataLock(long providerId, long dataLockEventId, string hashedApprenticeshipId, SubmitStatusViewModel submitStatusViewModel, string userId)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);
            var triage = _apprenticeshipMapper.MapTriangeStatus(submitStatusViewModel);

            _logger.Info($"Updating data lock to triage {triage} for datalock: {dataLockEventId}, apprenticeship: {apprenticeshipId}", apprenticeshipId);

            await _mediator.SendAsync(new UpdateDataLockCommand
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId,
                DataLockEventId = dataLockEventId,
                TriageStatus = triage,
                UserId = userId
            });
        }

        private async Task<List<ITrainingProgramme>> GetTrainingProgrammes()
        {
            var standardsTask = _mediator.SendAsync(new GetStandardsQueryRequest());
            var frameworksTask = _mediator.SendAsync(new GetFrameworksQueryRequest());

            await Task.WhenAll(standardsTask, frameworksTask);

            return
                standardsTask.Result.Standards.Cast<ITrainingProgramme>()
                    .Union(frameworksTask.Result.Frameworks)
                    .OrderBy(m => m.Title)
                    .ToList();
        }

        private async Task AssertNoPendingApprenticeshipUpdate(long providerId, long apprenticeshipId)
        {
            var result = await _mediator.SendAsync(new GetPendingApprenticeshipUpdateQueryRequest
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId
            });

            if (result.ApprenticeshipUpdate != null)
                throw new InvalidStateException("Pending apprenticeship update");
        }

        private void AssertApprenticeshipIsEditable(Apprenticeship apprenticeship)
        {
            var editable = new[] { PaymentStatus.Active, PaymentStatus.Paused, }.Contains(apprenticeship.PaymentStatus);
            if (!editable)
            {
                throw new FluentValidation.ValidationException("Unable to edit apprenticeship - not in active or paused state");
            }
        }

        private void AssertApprenticeshipUpdateReviewable(ApprenticeshipUpdate update)
        {
            if (update.Originator == Originator.Provider)
            {
                throw new FluentValidation.ValidationException("Unable to review a provider-originated update");
            }
        }
        private void AssertApprenticeshipUpdateUndoable(ApprenticeshipUpdate update)
        {
            if (update.Originator == Originator.Employer)
            {
                throw new FluentValidation.ValidationException("Unable to review a provider-originated update");
            }
        }

        public async Task TriageMultiplePriceDataLocks(long providerId, string hashedApprenticeshipId, string currentUserId, TriageStatus triageStatus)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info($"Updating price data locks to triage {triageStatus} for apprenticeship: {apprenticeshipId}", apprenticeshipId);

            await _mediator.SendAsync(new TriageApprenticeshipDataLocksCommand
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId,
                TriageStatus = triageStatus,
                UserId = currentUserId
            });
        }
    }
}