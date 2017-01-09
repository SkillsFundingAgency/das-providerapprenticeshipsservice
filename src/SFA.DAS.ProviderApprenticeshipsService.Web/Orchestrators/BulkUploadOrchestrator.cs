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

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public sealed class BulkUploadOrchestrator : BaseCommitmentOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly BulkUploader _bulkUploader;
        private readonly IHashingService _hashingService;
        private readonly IProviderCommitmentsLogger _logger;

        public BulkUploadOrchestrator(IMediator mediator, BulkUploader bulkUploader, IHashingService hashingService, IProviderCommitmentsLogger logger) : base (mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (bulkUploader == null)
                throw new ArgumentNullException(nameof(bulkUploader));
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _mediator = mediator;
            _bulkUploader = bulkUploader;
            _hashingService = hashingService;
            _logger = logger;
        }

        public async Task<BulkUploadResult> UploadFileAsync(UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel)
        {
            var commitmentId = _hashingService.DecodeValue(uploadApprenticeshipsViewModel.HashedCommitmentId);

            await AssertCommitmentStatus(commitmentId, uploadApprenticeshipsViewModel.ProviderId);

            _logger.Info($"Uploading file of apprentices. Filename:{uploadApprenticeshipsViewModel?.Attachment?.FileName}", providerId: uploadApprenticeshipsViewModel.ProviderId, commitmentId: commitmentId);
            
            var result = await _bulkUploader.UploadFile(uploadApprenticeshipsViewModel);

            var errorCount = result.Errors.Count();

            if (errorCount > 0)
            {
                _logger.Info($"{errorCount} Upload errors for Filename:{uploadApprenticeshipsViewModel?.Attachment?.FileName}", providerId: uploadApprenticeshipsViewModel.ProviderId, commitmentId: commitmentId);

                return result;
            }

            await _mediator.SendAsync(new BulkUploadApprenticeshipsCommand
            {
                ProviderId = uploadApprenticeshipsViewModel.ProviderId,
                CommitmentId = commitmentId,
                Apprenticeships = await MapFrom(commitmentId, result.Data)
            });

            return new BulkUploadResult { Errors = new List<UploadError>() };
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
    }
}