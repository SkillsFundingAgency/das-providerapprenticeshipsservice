using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.Commitments.Api.Types.Validation.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

using ApiTrainingType = SFA.DAS.Commitments.Api.Types.Apprenticeship.Types.TrainingType;

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

        public async Task<BulkUploadResultViewModel> UploadFile(string userId, UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel, SignInUserModel signInUser)
        {
            var result = new BulkUploadResultViewModel();

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

            var sw = Stopwatch.StartNew();
            var overlapErrors = await GetOverlapErrors(fileValidationResult.Data.ToList());
            _logger.Trace($"Validating overlaps took {sw.ElapsedMilliseconds}");

            var rowErrors = rowValidationResult.Errors.ToList();
            rowErrors.AddRange(overlapErrors);
            
            if (rowErrors.Any())
            {
                _logger.Info($"{rowErrors.Count} Upload errors", providerId, commitmentId);
                return new BulkUploadResultViewModel { HasRowLevelErrors = true, RowLevelErrors = rowErrors };
            }

            try
            {

                await _mediator.SendAsync(new BulkUploadApprenticeshipsCommand
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
                        HasRowLevelErrors = true,
                        RowLevelErrors = overlaps
                    };
                }
                else
                {
                    throw;
                }
            }

            return new BulkUploadResultViewModel();
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

            var overlapResponse = await _mediator.SendAsync(overlapRequest);

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
                apprenticeship.TrainingType = training is Standard ? ApiTrainingType.Standard : ApiTrainingType.Framework;
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

        private IEnumerable<UploadError> GetOverlappingErrors(OverlappingApprenticeship overlappingResult, int i, ApprenticeshipUploadModel record)
        {
            const string TextStartDate = "The <strong>start date</strong> overlaps with existing training dates for the same apprentice";
            const string TextEndDate = "The <strong>finish date</strong> overlaps with existing training dates for the same apprentice";

            switch (overlappingResult.ValidationFailReason)
            {
                case ValidationFailReason.OverlappingStartDate:
                    return new List<UploadError> { new UploadError(TextStartDate, "OverlappingError", i, record) };
                case ValidationFailReason.OverlappingEndDate:
                    return new List<UploadError> { new UploadError(TextEndDate, "OverlappingError", i, record) };
                case ValidationFailReason.DateEmbrace:
                case ValidationFailReason.DateWithin:
                    return new List<UploadError>
                               {
                                   new UploadError(TextStartDate, "OverlappingError", i, record),
                                   new UploadError(TextEndDate, "OverlappingError", i, record)
                               };
            }
            return Enumerable.Empty<UploadError>();
        }
    }
}