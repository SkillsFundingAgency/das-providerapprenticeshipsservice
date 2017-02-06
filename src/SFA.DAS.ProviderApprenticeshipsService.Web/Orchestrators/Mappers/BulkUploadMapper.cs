using System;
using System.Collections.Generic;
using System.Linq;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public class BulkUploadMapper
    {
        public Apprenticeship MapFrom(long commitmentId, ApprenticeshipViewModel viewModel, IList<ITrainingProgramme> trainingProgrammes)
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

        public List<UploadRowErrorViewModel> MapErrors(IEnumerable<UploadError> errors)
        {
            var result = errors
                ?.GroupBy(m => m.Row)
                .Select(MapError)
                .Where(m => m != null)
                .OrderBy(m => m.RowNumber)
                ?.ToList();

            return result;
        }

        private UploadRowErrorViewModel MapError(IGrouping<int?, UploadError> arg)
        {
            var firstRecord = arg.FirstOrDefault();
            Func<string, string> stringOrDefault = str => string.IsNullOrWhiteSpace(str) ? "&ndash;" : str;
            if (arg.Key != null && firstRecord != null)
            {
                return new UploadRowErrorViewModel
                           {
                               RowNumber = arg.Key.Value,
                               Name = $"{stringOrDefault(firstRecord.FirstName)} {stringOrDefault(firstRecord.LastName)}",
                               Uln = stringOrDefault(firstRecord.Uln),
                               DateOfBirth = stringOrDefault(firstRecord.DateOfBirth?.DateTime?.ToGdsFormat()),
                               Messages = arg.Select(m => m.Message)
                           };
            }

            return null;
        }
    }
}