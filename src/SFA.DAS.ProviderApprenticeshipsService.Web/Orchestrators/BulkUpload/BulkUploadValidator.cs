using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Ajax.Utilities;
using WebGrease.Css.Extensions;
using SFA.DAS.Learners.Validators;
using SFA.DAS.ProviderApprenticeshipsService.Application.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload
{
    public sealed class BulkUploadValidator : IBulkUploadValidator
    {
        private readonly ProviderApprenticeshipsServiceConfiguration _config;

        // TODO: LWA - Can these be injected in?
        private readonly BulkUploadApprenticeshipValidationText _validationText;
        private readonly ApprenticeshipUploadModelValidator _viewModelValidator;
       
        public BulkUploadValidator(ProviderApprenticeshipsServiceConfiguration config, IUlnValidator ulnValidator, IAcademicYearDateProvider academicYear)
        {
            _validationText = new BulkUploadApprenticeshipValidationText(academicYear);
            _viewModelValidator = new ApprenticeshipUploadModelValidator(_validationText, new CurrentDateTime(), ulnValidator);
            
            _config = config;
        }

        public IEnumerable<UploadError> ValidateFileSize(HttpPostedFileBase attachment)
        {
            var errors = new List<UploadError>();
            var maxFileSize = _config.MaxBulkUploadFileSize * 1024; // Bytes
            
            if (attachment.ContentLength > maxFileSize)
                errors.Add(new UploadError(ApprenticeshipFileValidationText.MaxFileSizeMessage(maxFileSize)));

            if(!attachment.FileName.ToLower().EndsWith(".csv"))
                errors.Add(new UploadError(ApprenticeshipFileValidationText.OnlyCsvFile));

            return errors;
        }

        public IEnumerable<UploadError> ValidateCohortReference(
            IEnumerable<ApprenticeshipUploadModel> records,
            string cohortReference)
        {
            var errors = new List<UploadError>();

            var apprenticeshipUploadModels = records as ApprenticeshipUploadModel[] ?? records.ToArray();
            if (!apprenticeshipUploadModels.Any()) return new[] { new UploadError(ApprenticeshipFileValidationText.NoRecords) };

            if (apprenticeshipUploadModels.Any(m => m.CsvRecord.CohortRef != apprenticeshipUploadModels.First().CsvRecord.CohortRef))
                errors.Add(new UploadError(_validationText.CohortRef01.Text.RemoveHtmlTags(), _validationText.CohortRef01.ErrorCode));

            if (apprenticeshipUploadModels.Any(m => m.CsvRecord.CohortRef != cohortReference))
                errors.Add(new UploadError(_validationText.CohortRef02.Text.RemoveHtmlTags(), _validationText.CohortRef02.ErrorCode));

            if (apprenticeshipUploadModels.Length != apprenticeshipUploadModels.DistinctBy(m => m.ApprenticeshipViewModel.ULN).Count())
                errors.Add(new UploadError(_validationText.Uln04.Text.RemoveHtmlTags(), _validationText.Uln04.ErrorCode));

            return errors;
        }

        public IEnumerable<UploadError> ValidateUlnUniqueness(IEnumerable<ApprenticeshipUploadModel> records)
        {
            var apprenticeshipUploadModels = records as ApprenticeshipUploadModel[] ?? records.ToArray();

            var result = new List<UploadError>();

            var distinctUlns = apprenticeshipUploadModels.Select(x => x.ApprenticeshipViewModel.ULN).Distinct().Count();

            if (apprenticeshipUploadModels.Count() != distinctUlns)
            {
                result.Add(new UploadError(_validationText.Uln04.Text.RemoveHtmlTags(), _validationText.Uln04.ErrorCode));
            }

            return result;
        }

        public IEnumerable<UploadError> ValidateRecords(IEnumerable<ApprenticeshipUploadModel> records, List<ITrainingProgramme> trainingProgrammes)
        {
            var errors = new ConcurrentBag<UploadError>();
            var apprenticeshipUploadModels = records as ApprenticeshipUploadModel[] ?? records.ToArray();
            
            Parallel.ForEach(apprenticeshipUploadModels,
                (record, state, index) =>
                    {
                        var viewModel = record.ApprenticeshipViewModel;
                        int i = (int)index + 1;

                        // Validate view model for approval
                        var validationResult = _viewModelValidator.Validate(record);
                        validationResult.Errors.ForEach(m => errors.Add(new UploadError(m.ErrorMessage, m.ErrorCode, i, record)));

                        //// TODO: LWA - Should we move this into the validator?
                        //if (!string.IsNullOrWhiteSpace(viewModel.TrainingCode) && trainingProgrammes.All(m => m.Id != viewModel.TrainingCode))
                        //    errors.Add(new UploadError("Not a valid <strong>Training code</strong>", "Training_01", i, record));

                        (string message, string errorCode) = ValidateTraining(viewModel, trainingProgrammes);
                        if (message != null)
                            errors.Add(new UploadError(message, errorCode, i, record));
                    });

            return errors;
        }

        private static (string message, string errorCode) ValidateTraining(ApprenticeshipViewModel viewModel, List<ITrainingProgramme> trainingProgrammes)
        {
            // we take our cue from the existing validation and only return the first error
            if (!string.IsNullOrWhiteSpace(viewModel.TrainingCode))
            {
                // not as safe as single, but quicker
                var trainingProgram = trainingProgrammes.Find(tp => tp.Id == viewModel.TrainingCode);
                if (trainingProgram == null)
                    return ("Not a valid <strong>Training code</strong>", "Training_01");
                    //errors.Add(new UploadError("Not a valid <strong>Training code</strong>", "Training_01", i, record));

                if (viewModel.StartDate?.DateTime != null)
                {
                    var courseStatus = trainingProgram.GetStatusOn(viewModel.StartDate.DateTime.Value);

                    if (courseStatus != TrainingProgrammeStatus.Active)
                    {
                        // if EffectiveFrom is null, then programme is valid to the big bang, so won't be pending, so we don't have to check for null (similar for expired also)
                        var suffix = courseStatus == TrainingProgrammeStatus.Pending
                            ? $"after {trainingProgram.EffectiveFrom.Value.AddMonths(-1):MM yyyy}"
                            : $"before {trainingProgram.EffectiveTo.Value.AddMonths(1):MM yyyy}";

                        return ($"This training course is only available to apprentices with a start date {suffix}", "Training_02");
                        //errors.Add(new UploadError(
                        //    $"This training course is only available to apprentices with a start date {suffix}", "Training_02",
                        //    i, record));
                    }
                }
            }

            return (null, null);
        }
    }
}