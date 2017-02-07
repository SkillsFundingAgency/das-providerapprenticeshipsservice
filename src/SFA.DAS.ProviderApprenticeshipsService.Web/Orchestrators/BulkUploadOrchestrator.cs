using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public sealed class BulkUploadOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly BulkUploader _bulkUploader;
        private readonly IHashingService _hashingService;

        private readonly BulkUploadMapper _mapper;

        private readonly IProviderCommitmentsLogger _logger;

        public BulkUploadOrchestrator(
            IMediator mediator,
            BulkUploader bulkUploader, 
            IHashingService hashingService,
            BulkUploadMapper mapper,
            IProviderCommitmentsLogger logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (bulkUploader == null)
                throw new ArgumentNullException(nameof(bulkUploader));
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _mediator = mediator;
            _bulkUploader = bulkUploader;
            _hashingService = hashingService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BulkUploadResultViewModel> UploadFile(UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel)
        {
            var commitmentId = _hashingService.DecodeValue(uploadApprenticeshipsViewModel.HashedCommitmentId);
            var providerId = uploadApprenticeshipsViewModel.ProviderId;
            var fileName = uploadApprenticeshipsViewModel?.Attachment?.FileName ?? "<unknown>";

            _logger.Info($"Uploading File - Filename:{fileName}", uploadApprenticeshipsViewModel.ProviderId, commitmentId);

            var fileValidationResult = _bulkUploader.ValidateFileStructure(uploadApprenticeshipsViewModel, fileName, commitmentId);

            if (fileValidationResult.Errors.Any())
            {
                return new BulkUploadResultViewModel { HasFileLevelErrors = true, FileLevelErrors = fileValidationResult.Errors };
            }

            _logger.Info("Uploading file of apprentices.", providerId, commitmentId);

            var rowValidationResult = await _bulkUploader.ValidateFileRows(fileValidationResult, providerId);

            var errorCount = rowValidationResult.Errors.Count();
            if (errorCount > 0)
            {
                _logger.Info($"{errorCount} Upload errors for", providerId, commitmentId);
                return new BulkUploadResultViewModel { HasRowLevelErrors = true, RowLevelErrors = rowValidationResult.Errors };
            }

            await _mediator.SendAsync(new BulkUploadApprenticeshipsCommand
            {
                ProviderId = providerId,
                CommitmentId = commitmentId,
                Apprenticeships = await _mapper.MapFrom(commitmentId, rowValidationResult.Data)
            });

            return new BulkUploadResultViewModel();
        }

        public async Task<UploadApprenticeshipsViewModel> GetUploadModel(long providerid, string hashedcommitmentid)
        {
            var commitmentId = _hashingService.DecodeValue(hashedcommitmentid);
            var result = await _mediator.SendAsync(new GetCommitmentQueryRequest
                                              {
                                                  ProviderId = providerid,
                                                  CommitmentId = commitmentId
                                              });

            var model = new UploadApprenticeshipsViewModel
            {
                ProviderId = providerid,
                HashedCommitmentId = hashedcommitmentid,
                ApprenticeshipCount = result.Commitment.Apprenticeships.Count
            };

            return model;
        }

        public UploadApprenticeshipsViewModel GetUnsuccessfulUpload(List<UploadError> errors, long providerId, string hashedCommitmentId)
        {
            var result = _mapper.MapErrors(errors);
            var fileErrors = errors.Where(m => m.IsGeneralError);

            return new UploadApprenticeshipsViewModel
            {
                ProviderId = providerId,
                HashedCommitmentId = hashedCommitmentId,
                ErrorCount = errors.Count,
                RowCount = result.Count,
                Errors = result,
                FileErrors = fileErrors
            };
        }
    }
}