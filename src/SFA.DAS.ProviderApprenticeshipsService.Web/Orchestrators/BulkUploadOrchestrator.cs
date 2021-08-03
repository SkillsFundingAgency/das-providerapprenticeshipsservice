using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.Commitments.Api.Types.Validation.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetBulkUploadFile;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Extensions;


namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public sealed class BulkUploadOrchestrator : BaseCommitmentOrchestrator
    {
        private readonly BulkUploader _bulkUploader;
        private readonly BulkUploadMapper _mapper;
        private readonly IBulkUploadFileParser _fileParser;
        private readonly IReservationsService _reservationsService;

        public BulkUploadOrchestrator(
            IMediator mediator,
            BulkUploader bulkUploader, 
            IHashingService hashingService,
            BulkUploadMapper mapper,
            IProviderCommitmentsLogger logger,
            IBulkUploadFileParser fileParser,
            IReservationsService reservationsService) : base(mediator, hashingService, logger)
        {
            _bulkUploader = bulkUploader;
            _mapper = mapper;
            _fileParser = fileParser;
            _reservationsService = reservationsService ?? throw new ArgumentNullException(nameof(reservationsService));
        }

        public async Task<BulkUploadResultViewModel> UploadFile(string userId, UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel, SignInUserModel signInUser)
        {
            var commitmentId = HashingService.DecodeValue(uploadApprenticeshipsViewModel.HashedCommitmentId);
            var providerId = uploadApprenticeshipsViewModel.ProviderId;
            var fileName = uploadApprenticeshipsViewModel.Attachment?.FileName ?? "<unknown>";  

            var commitment = await GetCommitment(providerId, commitmentId);
            AssertCommitmentStatus(commitment);
            await AssertAutoReservationEnabled(commitment);

            Logger.Info($"Uploading File - Filename:{fileName}", uploadApprenticeshipsViewModel.ProviderId, commitmentId);

            var fileValidationResult = await _bulkUploader.ValidateFileStructure(uploadApprenticeshipsViewModel, providerId, commitment);

            if (fileValidationResult.Errors.Any())
            {
                return new BulkUploadResultViewModel
                {
                    BulkUploadId = fileValidationResult.BulkUploadId,
                    HasFileLevelErrors = true,
                    FileLevelErrors = fileValidationResult.Errors
                };
            }

            Logger.Info("Uploading file of apprentices.", providerId, commitmentId);

            var rowValidationResult = await _bulkUploader.ValidateFileRows(fileValidationResult.Data, providerId, fileValidationResult.BulkUploadId);

            var sw = Stopwatch.StartNew();
            var overlapErrors = await GetOverlapErrors(fileValidationResult.Data.ToList());
            Logger.Trace($"Validating overlaps took {sw.ElapsedMilliseconds}");

            var rowErrors = rowValidationResult.Errors.ToList();
            rowErrors.AddRange(overlapErrors);
            var hashedBulkUploadId = HashingService.HashValue(fileValidationResult.BulkUploadId);
            if (rowErrors.Any())
            {
                Logger.Info($"{rowErrors.Count} Upload errors", providerId, commitmentId);
                return new BulkUploadResultViewModel
                {
                    BulkUploadId = fileValidationResult.BulkUploadId,
                    BulkUploadReference = hashedBulkUploadId,
                    HasRowLevelErrors = true,
                    RowLevelErrors = rowErrors
                };
            }

            try
            {

                await Mediator.Send(new BulkUploadApprenticeshipsCommand
                {
                    UserId = userId,
                    ProviderId = providerId,
                    CommitmentId = commitmentId,
                    Apprenticeships = await _mapper.MapFrom(commitmentId, rowValidationResult.Data),
                    UserEmailAddress = signInUser.Email,
                    UserDisplayName = signInUser.DisplayName
                });
            }
            catch (Exception)
            {
                var overlaps = (await GetOverlapErrors(fileValidationResult.Data.ToList())).ToList();
                if (overlaps.Any())
                {
                    return new BulkUploadResultViewModel
                    {
                        BulkUploadId = fileValidationResult.BulkUploadId,
                        HasRowLevelErrors = true,
                        RowLevelErrors = overlaps
                    };
                }

                throw;
            }

            return new BulkUploadResultViewModel { BulkUploadId = fileValidationResult.BulkUploadId };
        }

        private async Task<IEnumerable<UploadError>> GetOverlapErrors(IList<ApprenticeshipUploadModel> uploadedApprenticeships)
        {
            var result = new List<UploadError>();

            var apprentices = new List<Apprenticeship>();

            var i = 0;
            foreach (var apprentice in uploadedApprenticeships.Where(x=> 
                !string.IsNullOrWhiteSpace(x.ApprenticeshipViewModel.ULN)
                && x.ApprenticeshipViewModel.StartDate.DateTime.HasValue
                && x.ApprenticeshipViewModel.EndDate.DateTime.HasValue
                ))
            {
                apprentices.Add(new Apprenticeship
                {
                    Id = i, //assign a row id, as this value will be zero for files
                    ULN = apprentice.ApprenticeshipViewModel.ULN,
                    StartDate = apprentice.ApprenticeshipViewModel.StartDate.DateTime.Value,
                    EndDate = apprentice.ApprenticeshipViewModel.EndDate.DateTime.Value
                });
                i++;
            }

            var overlapRequest = new GetOverlappingApprenticeshipsQueryRequest
            {
                Apprenticeship = apprentices
            };

            var overlapResponse = await Mediator.Send(overlapRequest);

            if (overlapResponse.Overlaps.Any())
            {
                
                var validationErrors = uploadedApprenticeships.ToList();

                foreach (var overlapGroup in overlapResponse.Overlaps)
                {
                    foreach (var overlap in overlapGroup.OverlappingApprenticeships.ToList())
                    {
                        var row = validationErrors.Single(x => x.ApprenticeshipViewModel.ULN == overlap.Apprenticeship.ULN);
                        var rowIndex = validationErrors.IndexOf(row) + 1;

                        var e = GetOverlappingErrors(overlap, rowIndex, row);

                        result.AddRange(e);
                    }
                }
            }
            return result;
        }

        public async Task<UploadApprenticeshipsViewModel> GetUploadModel(long providerid, string hashedcommitmentid)
        {
            var commitment = await GetCommitment(providerid, hashedcommitmentid);
            AssertCommitmentStatus(commitment);
            await AssertAutoReservationEnabled(commitment);
            AssertIsNotChangeOfParty(commitment);

            return new UploadApprenticeshipsViewModel
            {
                ProviderId = providerid,
                HashedCommitmentId = hashedcommitmentid,
                ApprenticeshipCount = commitment.Apprenticeships.Count,
                IsPaidByTransfer = commitment.IsTransfer(),
                AccountLegalEntityPublicHashedId = commitment.AccountLegalEntityPublicHashedId //AgreementId
            };
        }

        public async Task<UploadApprenticeshipsViewModel> GetUnsuccessfulUpload(long providerId, string hashedCommitmentId, string bulkUploadReference)
        {
            var commitmentId = HashingService.DecodeValue(hashedCommitmentId);
            var bulkUploadId = HashingService.DecodeValue(bulkUploadReference);

            var commitment = await GetCommitment(providerId, commitmentId);
            AssertCommitmentStatus(commitment);
            await AssertAutoReservationEnabled(commitment);

            var fileContentResult = await Mediator.Send(new GetBulkUploadFileQueryRequest
            {
                ProviderId = providerId,
                BulkUploadId = bulkUploadId
            });

            var uploadResult = _fileParser.CreateViewModels(providerId, commitment, fileContentResult.FileContent);

            var validationResult = await _bulkUploader.ValidateFileRows(uploadResult.Data, providerId, bulkUploadId);
            var overlaps = await GetOverlapErrors(uploadResult.Data.ToList());

            var errors = validationResult.Errors.ToList();
            errors.AddRange(overlaps);

            var result = _mapper.MapErrors(errors);

            return new UploadApprenticeshipsViewModel
            {
                ProviderId = providerId,
                HashedCommitmentId = hashedCommitmentId,
                ErrorCount = errors.Count,
                RowCount = result.Count,
                Errors = result,
                FileErrors = new List<UploadError>()
            };
        }

        private IEnumerable<UploadError> GetOverlappingErrors(OverlappingApprenticeship overlappingResult, int i, ApprenticeshipUploadModel record)
        {
            const string textStartDate = "The <strong>start date</strong> overlaps with existing training dates for the same apprentice";
            const string textEndDate = "The <strong>finish date</strong> overlaps with existing training dates for the same apprentice";

            switch (overlappingResult.ValidationFailReason)
            {
                case ValidationFailReason.OverlappingStartDate:
                    return new List<UploadError> { new UploadError(textStartDate, "OverlappingError", i, record) };
                case ValidationFailReason.OverlappingEndDate:
                    return new List<UploadError> { new UploadError(textEndDate, "OverlappingError", i, record) };
                case ValidationFailReason.DateEmbrace:
                case ValidationFailReason.DateWithin:
                    return new List<UploadError>
                               {
                                   new UploadError(textStartDate, "OverlappingError", i, record),
                                   new UploadError(textEndDate, "OverlappingError", i, record)
                               };
            }
            return Enumerable.Empty<UploadError>();
        }

        private async Task AssertAutoReservationEnabled(CommitmentView commitment)
        {
            if (!await _reservationsService.IsAutoReservationEnabled(commitment.EmployerAccountId, commitment.TransferSender?.Id))
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, "Current account is not authorized for automatic reservations");

            }
        }

        private void AssertIsNotChangeOfParty(CommitmentView commitment)
        {
            if (commitment.IsLinkedToChangeOfPartyRequest)
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, "Cohort is linked to a ChangeOfParty Request - apprentices cannot be added to it");
            }
        }
    }
}