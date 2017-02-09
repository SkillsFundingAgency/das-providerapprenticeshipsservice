using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public sealed class BulkUploadOrchestrator : BaseCommitmentOrchestrator
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
            IProviderCommitmentsLogger logger) : base(mediator)
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

			await AssertCommitmentStatus(commitmentId, uploadApprenticeshipsViewModel.ProviderId);
            _logger.Info($"Uploading File - Filename:{fileName}", uploadApprenticeshipsViewModel.ProviderId, commitmentId);

            var fileValidationResult = _bulkUploader.ValidateFileStructure(uploadApprenticeshipsViewModel, fileName, commitmentId);

            if (fileValidationResult.Errors.Any())
            {
                return new BulkUploadResultViewModel { HasFileLevelErrors = true, FileLevelErrors = fileValidationResult.Errors };
            }

            _logger.Info("Uploading file of apprentices.", providerId, commitmentId);

            var rowValidationResult = await _bulkUploader.ValidateFileRows(fileValidationResult.Data, providerId);

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
		
		private async Task<IList<Apprenticeship>> MapFrom(long commitmentId, IEnumerable<ApprenticeshipUploadModel> data)
        {
            var trainingProgrammes = await GetTrainingProgrammes();

            return data.Select(x => MapFrom(commitmentId, x.ApprenticeshipViewModel, trainingProgrammes)).ToList();
        }

        private Apprenticeship MapFrom(long commitmentId, ApprenticeshipViewModel viewModel, IList<ITrainingProgramme> trainingProgrammes)
        {
            var apprenticeship = new Apprenticeship
            {
                CommitmentId = commitmentId,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                DateOfBirth = viewModel.DateOfBirth.DateTime,
                NINumber = viewModel.NINumber,
                ULN = viewModel.ULN,
                Cost = viewModel.Cost == null ? default(decimal?) : decimal.Parse(viewModel.Cost),
                StartDate = viewModel.StartDate.DateTime,
                EndDate = viewModel.EndDate.DateTime,
                ProviderRef = viewModel.ProviderRef
            };

            if (!string.IsNullOrWhiteSpace(viewModel.TrainingCode))
            {
                var training = trainingProgrammes.Single(x => x.Id == viewModel.TrainingCode);
                apprenticeship.TrainingType = training is Standard ? Commitments.Api.Types.TrainingType.Standard : Commitments.Api.Types.TrainingType.Framework;
                apprenticeship.TrainingCode = viewModel.TrainingCode;
                apprenticeship.TrainingName = training.Title;
            }

            return apprenticeship;
        }

        //TODO: These are duplicated in Commitment Orchestrator - needs to be shared
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

        public async Task<UploadApprenticeshipsViewModel> GetUploadModel(long providerid, string hashedcommitmentid)
        {
            var commitmentId = _hashingService.DecodeValue(hashedcommitmentid);
            await AssertCommitmentStatus(commitmentId, providerid);
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

        public async Task<UploadApprenticeshipsViewModel> GetUnsuccessfulUpload(List<UploadError> errors, long providerId, string hashedCommitmentId)
        {

            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            await AssertCommitmentStatus(commitmentId, providerId);
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