using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.Commitments.Api.Types.Validation.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetBulkUploadFile;
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
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public sealed class BulkUploadOrchestrator : BaseCommitmentOrchestrator
    {
        private readonly BulkUploader _bulkUploader;
        private readonly BulkUploadMapper _mapper;
        private readonly IBulkUploadFileParser _fileParser;

        public BulkUploadOrchestrator(
            IMediator mediator,
            BulkUploader bulkUploader, 
            IHashingService hashingService,
            BulkUploadMapper mapper,
            IProviderCommitmentsLogger logger,
            IBulkUploadFileParser fileParser) : base(mediator, hashingService, logger)
        {
            _bulkUploader = bulkUploader;
            _mapper = mapper;
            _fileParser = fileParser;
        }

        public async Task<BulkUploadResultViewModel> UploadFile(string userId, UploadApprenticeshipsViewModel uploadApprenticeshipsViewModel, SignInUserModel signInUser)
        {
            var commitmentId = HashingService.DecodeValue(uploadApprenticeshipsViewModel.HashedCommitmentId);
            var providerId = uploadApprenticeshipsViewModel.ProviderId;
            var fileName = uploadApprenticeshipsViewModel?.Attachment?.FileName ?? "<unknown>";

			await AssertCommitmentStatus(commitmentId, uploadApprenticeshipsViewModel.ProviderId);
            Logger.Info($"Uploading File - Filename:{fileName}", uploadApprenticeshipsViewModel.ProviderId, commitmentId);

            var fileValidationResult = await _bulkUploader.ValidateFileStructure(uploadApprenticeshipsViewModel, providerId, commitmentId);

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

                await Mediator.SendAsync(new BulkUploadApprenticeshipsCommand
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

            var overlapResponse = await Mediator.SendAsync(overlapRequest);

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
            var standardsTask = Mediator.SendAsync(new GetStandardsQueryRequest());
            var frameworksTask = Mediator.SendAsync(new GetFrameworksQueryRequest());

            await Task.WhenAll(standardsTask, frameworksTask);

            return
                standardsTask.Result.Standards.Cast<ITrainingProgramme>()
                    .Union(frameworksTask.Result.Frameworks)
                    .OrderBy(m => m.Title)
                    .ToList();
        }

        public async Task<UploadApprenticeshipsViewModel> GetUploadModel(long providerid, string hashedcommitmentid)
        {
            var commitment = await GetCommitment(providerid, hashedcommitmentid);
            AssertCommitmentStatus(commitment);

            AssertCohortNotPaidForByTransfer(commitment);

            return new UploadApprenticeshipsViewModel
            {
                ProviderId = providerid,
                HashedCommitmentId = hashedcommitmentid,
                ApprenticeshipCount = commitment.Apprenticeships.Count
            };
        }

        public async Task<UploadApprenticeshipsViewModel> GetUnsuccessfulUpload(long providerId, string hashedCommitmentId, string bulkUploadReference)
        {
            var commitmentId = HashingService.DecodeValue(hashedCommitmentId);
            var bulkUploadId = HashingService.DecodeValue(bulkUploadReference);

            await AssertCommitmentStatus(commitmentId, providerId);

            var fileContentResult = await Mediator.SendAsync(new GetBulkUploadFileQueryRequest
            {
                ProviderId = providerId,
                BulkUploadId = bulkUploadId
            });

            var uploadResult = _fileParser.CreateViewModels(providerId, commitmentId, fileContentResult.FileContent);

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

        private void AssertCohortNotPaidForByTransfer(CommitmentView commitment)
        {
            if (commitment.IsTransfer())
                throw new InvalidOperationException("Bulk upload disabled for commitment paid for by a transfer");
        }
    }
}