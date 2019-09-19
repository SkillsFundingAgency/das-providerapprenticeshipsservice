using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.TriageApprenticeshipDataLocks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateDataLock;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLockSummary;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipPriceHistory;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public sealed class DataLockOrchestrator : BaseCommitmentOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;
        private readonly IProviderCommitmentsLogger _logger;
        private readonly IApprenticeshipMapper _apprenticeshipMapper;
        private readonly IDataLockMapper _dataLockMapper;

        public DataLockOrchestrator(
            IMediator mediator,
            IHashingService hashingService,
            IProviderCommitmentsLogger logger,
            IApprenticeshipMapper apprenticeshipMapper,
            IDataLockMapper dataLockMapper) : base(mediator, hashingService, logger)
        {
            _mediator = mediator;
            _hashingService = hashingService;
            _logger = logger;
            _apprenticeshipMapper = apprenticeshipMapper;
            _dataLockMapper = dataLockMapper;
        }

        public async Task<DataLockMismatchViewModel> GetApprenticeshipMismatchDataLock(long providerId, string hashedApprenticeshipId)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info($"Getting apprenticeship datalock for provider: {providerId}", providerId: providerId, apprenticeshipId: apprenticeshipId);

            var datalockSummary = await _mediator.Send(new GetApprenticeshipDataLockSummaryQueryRequest
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId
            });

            var data = await _mediator.Send(new GetApprenticeshipQueryRequest
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId
            });

            var commitmentData = await _mediator.Send(new GetCommitmentQueryRequest
            {
                ProviderId = providerId,
                CommitmentId = data.Apprenticeship.CommitmentId
            });

            var priceHistory = await _mediator.Send(new GetApprenticeshipPriceHistoryQueryRequest
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId
            });

            var datalockSummaryViewModel = await _dataLockMapper.MapDataLockSummary(datalockSummary.DataLockSummary, data.Apprenticeship.HasHadDataLockSuccess);

            var dasRecordViewModel = _apprenticeshipMapper.MapApprenticeship(data.Apprenticeship, commitmentData.Commitment);
            var priceDataLocks = datalockSummaryViewModel
                .DataLockWithCourseMismatch
                .Concat(datalockSummaryViewModel.DataLockWithOnlyPriceMismatch)
                .Where(m => m.DataLockErrorCode.HasFlag(DataLockErrorCode.Dlock07))
                .OrderBy(x => x.IlrEffectiveFromDate);

            return new DataLockMismatchViewModel
            {
                ProviderId = providerId,
                HashedApprenticeshipId = hashedApprenticeshipId,
                DasApprenticeship = dasRecordViewModel,
                DataLockSummaryViewModel = datalockSummaryViewModel,
                EmployerName = data.Apprenticeship.LegalEntityName,
                PriceDataLocks = _dataLockMapper.MapPriceDataLock(priceHistory.History, priceDataLocks),
                CourseDataLocks = _dataLockMapper.MapCourseDataLock(dasRecordViewModel, datalockSummaryViewModel.DataLockWithCourseMismatch, data.Apprenticeship, priceHistory.History)
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

        public async Task TriageMultiplePriceDataLocks(long providerId, string hashedApprenticeshipId, string currentUserId, TriageStatus triageStatus)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info($"Updating price data locks to triage {triageStatus} for apprenticeship: {apprenticeshipId}", apprenticeshipId);

            await _mediator.Send(new TriageApprenticeshipDataLocksCommand
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId,
                TriageStatus = triageStatus,
                UserId = currentUserId
            });
        }

        public async Task UpdateDataLock(long providerId, long dataLockEventId, string hashedApprenticeshipId, SubmitStatusViewModel submitStatusViewModel, string userId)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);
            var triage = _apprenticeshipMapper.MapTriangeStatus(submitStatusViewModel);

            _logger.Info($"Updating data lock to triage {triage} for datalock: {dataLockEventId}, apprenticeship: {apprenticeshipId}", apprenticeshipId);

            await _mediator.Send(new UpdateDataLockCommand
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId,
                TriageStatus = triage,
                UserId = userId
            });
        }

        public async Task RequestRestart(long providerId, long dataLockEventId, string hashedApprenticeshipId, string userId)
        {
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            await _mediator.Send(new UpdateDataLockCommand
            {
                ProviderId = providerId,
                ApprenticeshipId = apprenticeshipId,
                TriageStatus = TriageStatus.Restart,
                UserId = userId
            });
        }
    }
}