using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
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

        public BulkUploadResult GetFile(UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel)
        {
            var commitmentId = _hashingService.DecodeValue(uploadApprenticeshipsViewModel.HashedCommitmentId);

            _logger.Info($"Get file. Filename:{uploadApprenticeshipsViewModel?.Attachment?.FileName}", uploadApprenticeshipsViewModel.ProviderId, commitmentId);

            var result = _bulkUploader.ValidateFile(uploadApprenticeshipsViewModel);

            return result;
        }

        public async Task<BulkUploadResult> UploadFileAsync(BulkUploadResult results, string hashedCommitmentId, long providerId)
        {
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info("Uploading file of apprentices.", providerId, commitmentId);
            
            var result = await _bulkUploader.Validate(results, hashedCommitmentId, providerId);

            var errorCount = result.Errors.Count();
            if (errorCount > 0)
            {
                _logger.Info($"{errorCount} Upload errors for", providerId, commitmentId);
                return result;
            }

            await _mediator.SendAsync(new BulkUploadApprenticeshipsCommand
            {
                ProviderId = providerId,
                CommitmentId = commitmentId,
                Apprenticeships = await _mapper.MapFrom(commitmentId, result.Data)
            });

            return new BulkUploadResult { Errors = new List<UploadError>() };
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