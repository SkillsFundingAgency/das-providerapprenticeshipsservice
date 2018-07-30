using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.ReviewApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UndoApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.ApprenticeshipSearch;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLocks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLockSummary;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetPendingApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public sealed class ManageApprenticesOrchestrator : BaseCommitmentOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IProviderCommitmentsLogger _logger;
        private readonly IHashingService _hashingService;
        private readonly IApprenticeshipMapper _apprenticeshipMapper;
        private readonly IApprovedApprenticeshipValidator _approvedApprenticeshipValidator;
        private readonly IApprenticeshipFiltersMapper _apprenticeshipFiltersMapper;
        private readonly IDataLockMapper _dataLockMapper;
        private readonly string _searchPlaceholderText;

        public ManageApprenticesOrchestrator(
            IMediator mediator,
            IHashingService hashingService,
            IProviderCommitmentsLogger logger,
            IApprenticeshipMapper apprenticeshipMapper,
            IApprovedApprenticeshipValidator approvedApprenticeshipValidator,
            IApprenticeshipFiltersMapper apprenticeshipFiltersMapper,
            IDataLockMapper dataLockMapper) : base(mediator, hashingService, logger)
        {
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

            result.DataLockSummaryViewModel = await _dataLockMapper.MapDataLockSummary(dataLockSummary.DataLockSummary, data.Apprenticeship.HasHadDataLockSuccess);

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

            var commitmentData = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                ProviderId = providerId,
                CommitmentId = data.Apprenticeship.CommitmentId
            });

            var overlappingErrors = await _mediator.SendAsync(
                new GetOverlappingApprenticeshipsQueryRequest
                {
                    Apprenticeship = new List<Apprenticeship> { data.Apprenticeship }
                });

            var dataLocks = await _mediator.SendAsync(
                    new GetApprenticeshipDataLocksQueryRequest
                        {
                            ApprenticeshipId = apprenticeshipId,
                            ProviderId = providerId
                        });

            var apprenticeship = _apprenticeshipMapper.MapApprenticeship(data.Apprenticeship, commitmentData.Commitment);

            return new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = apprenticeship,
                ApprenticeshipProgrammes = await GetTrainingProgrammes(),
                ValidationErrors = _approvedApprenticeshipValidator.MapOverlappingErrors(overlappingErrors)
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

            foreach (var overlap in _approvedApprenticeshipValidator.MapOverlappingErrors(overlappingErrors))
            {
                result.Add(overlap.Key, overlap.Value);
            }

            foreach (var error in _approvedApprenticeshipValidator.ValidateToDictionary(model))
            {
                result.AddIfNotExists(error.Key, error.Value);
            }

            foreach (var error in _approvedApprenticeshipValidator.ValidateAcademicYear(updateViewModel))
            {
                result.AddIfNotExists(error.Key, error.Value);
            }

            foreach (var error in _approvedApprenticeshipValidator.ValidateApprovedEndDate(updateViewModel))
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
    }
}