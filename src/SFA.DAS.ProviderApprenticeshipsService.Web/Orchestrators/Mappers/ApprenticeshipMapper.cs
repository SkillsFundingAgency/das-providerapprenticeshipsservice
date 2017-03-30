using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Validation.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using TrainingType = SFA.DAS.ProviderApprenticeshipsService.Domain.TrainingType;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public class ApprenticeshipMapper : IApprenticeshipMapper
    {
        private readonly IHashingService _hashingService;
        private readonly IMediator _mediator;

        public ApprenticeshipMapper(IHashingService hashingService, IMediator mediator)
        {
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));
            if(mediator==null)
                throw new ArgumentNullException(nameof(mediator));

            _hashingService = hashingService;
            _mediator = mediator;
        }

        public ApprenticeshipViewModel MapToApprenticeshipViewModel(Apprenticeship apprenticeship)
        {
            var dateOfBirth = apprenticeship.DateOfBirth;
            return new ApprenticeshipViewModel
            {
                HashedApprenticeshipId = _hashingService.HashValue(apprenticeship.Id),
                HashedCommitmentId = _hashingService.HashValue(apprenticeship.CommitmentId),
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                DateOfBirth = new DateTimeViewModel(dateOfBirth?.Day, dateOfBirth?.Month, dateOfBirth?.Year),
                NINumber = apprenticeship.NINumber,
                ULN = apprenticeship.ULN,
                TrainingType = apprenticeship.TrainingType,
                TrainingCode = apprenticeship.TrainingCode,
                Cost = NullableDecimalToString(apprenticeship.Cost),
                StartDate = new DateTimeViewModel(apprenticeship.StartDate),
                EndDate = new DateTimeViewModel(apprenticeship.EndDate),
                PaymentStatus = apprenticeship.PaymentStatus,
                AgreementStatus = apprenticeship.AgreementStatus,
                ProviderRef = apprenticeship.ProviderRef,
                EmployerRef = apprenticeship.EmployerRef
            };
        }
        private static string NullableDecimalToString(decimal? item)
        {
            return (item.HasValue) ? string.Format("{0:#}", item.Value) : "";
        }
        
        public Dictionary<string, string> MapOverlappingErrors(GetOverlappingApprenticeshipsQueryResponse overlappingErrors)
        {
            var dict = new Dictionary<string, string>();
            const string StartText = "The start date is not valid";
            const string EndText = "The end date is not valid";

            const string StartDateKey = "StartDateOverlap";
            const string EndDateKey = "EndDateOverlap";


            foreach (var item in overlappingErrors.GetFirstOverlappingApprenticeships())
            {
                switch (item.ValidationFailReason)
                {
                    case ValidationFailReason.OverlappingStartDate:
                        dict.AddIfNotExists(StartDateKey, StartText);
                        break;
                    case ValidationFailReason.OverlappingEndDate:
                        dict.AddIfNotExists(EndDateKey, EndText);
                        break;
                    case ValidationFailReason.DateEmbrace:
                        dict.AddIfNotExists(StartDateKey, StartText);
                        dict.AddIfNotExists(EndDateKey, EndText);
                        break;
                    case ValidationFailReason.DateWithin:
                        dict.AddIfNotExists(StartDateKey, StartText);
                        dict.AddIfNotExists(EndDateKey, EndText);
                        break;
                }
            }
            return dict;
        }

        public async Task<UpdateApprenticeshipViewModel> CompareAndMapToUpdateApprenticeshipViewModel(Apprenticeship original, ApprenticeshipViewModel edited)
        {
            Func<string, string, string> changedOrNull = (a, edit) =>
               a?.Trim() == edit?.Trim() ? null : edit;

            // ToDo: The rest of the mapping
            var model = new UpdateApprenticeshipViewModel
            {
                FirstName = changedOrNull(original.FirstName, edited.FirstName),
                LastName = changedOrNull(original.LastName, edited.LastName),
                DateOfBirth = original.DateOfBirth == edited.DateOfBirth.DateTime
                    ? null
                    : edited.DateOfBirth,
                Cost = NullableDecimalToString(original.Cost) == edited.Cost
                    ? default(decimal?)
                    : string.IsNullOrEmpty(edited.Cost) ? 0m : decimal.Parse(edited.Cost),
                StartDate = original.StartDate == edited.StartDate.DateTime
                  ? null
                  : edited.StartDate,
                EndDate = original.EndDate == edited.EndDate.DateTime
                    ? null
                    : edited.EndDate,
                EmployerRef = changedOrNull(original.EmployerRef, edited.EmployerRef),
                OriginalApprenticeship = original
            };

            if (!string.IsNullOrWhiteSpace(edited.TrainingCode) && original.TrainingCode != edited.TrainingCode)
            {
                var training = await GetTrainingProgramme(edited.TrainingCode);
                model.TrainingType = training is Standard ? TrainingType.Standard : TrainingType.Framework;
                model.TrainingCode = edited.TrainingCode;
                model.TrainingName = training.Title;
            }

            return model;
        }



        private async Task<ITrainingProgramme> GetTrainingProgramme(string trainingCode)
        {
            return (await GetTrainingProgrammes()).Single(x => x.Id == trainingCode);
        }

        private async Task<List<ITrainingProgramme>> GetTrainingProgrammes()
        {
            var standardsTask = _mediator.SendAsync(new GetStandardsQueryRequest());
            var frameworksTask = _mediator.SendAsync(new GetFrameworksQueryRequest());

            await Task.WhenAll(standardsTask, frameworksTask);

            return
                standardsTask.Result.Standards.Cast<ITrainingProgramme>()
                    .Union(frameworksTask.Result.Frameworks.Cast<ITrainingProgramme>())
                    .OrderBy(m => m.Title)
                    .ToList();
        }

        public ApprenticeshipUpdate MapFrom(UpdateApprenticeshipViewModel viewModel)
        {
            return new ApprenticeshipUpdate
            {
                ApprenticeshipId = viewModel.OriginalApprenticeship.Id,
                Cost = viewModel.Cost,
                DateOfBirth = viewModel.DateOfBirth?.DateTime,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                StartDate = viewModel.StartDate?.DateTime,
                EndDate = viewModel.EndDate?.DateTime,
                Originator = Originator.Provider,
                Status = ApprenticeshipUpdateStatus.Pending,
                TrainingName = viewModel.TrainingName,
                TrainingCode = viewModel.TrainingCode,
                TrainingType = (Commitments.Api.Types.Apprenticeship.Types.TrainingType?) viewModel.TrainingType,
            };
        }
    }
}