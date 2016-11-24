﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitments;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.Tasks.Api.Types.Templates;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    using System.Globalization;

    using WebGrease.Css.Extensions;

    public class CommitmentOrchestrator
    {
        private readonly IMediator _mediator;

        public CommitmentOrchestrator(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
        }

        public async Task<CommitmentListViewModel> GetAll(long providerId)
        {
            var data = await _mediator.SendAsync(new GetCommitmentsQueryRequest
            {
                ProviderId = providerId
            });

            var tasks = await _mediator.SendAsync(new GetTasksQueryRequest
            {
                ProviderId = providerId
            });

            return new CommitmentListViewModel
            {
                ProviderId = providerId,
                NumberOfTasks = tasks.Tasks.Count,
                Commitments = data.Commitments
            };
        }

        public async Task<CommitmentViewModel> Get(long providerId, long commitmentId)
        {
            var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                ProviderId = providerId,
                CommitmentId = commitmentId
            });

            var allTasks = await _mediator.SendAsync(new GetTasksQueryRequest { ProviderId = providerId });

            var taskForCommitment = allTasks?.Tasks
                .Select(x => new { Task = JsonConvert.DeserializeObject<CreateCommitmentTemplate>(x.Body), CreateDate = x.CreatedOn })
                .Where(x => x.Task != null && x.Task.CommitmentId == commitmentId)
                .OrderByDescending(x => x.CreateDate)
                .FirstOrDefault();

            var message = taskForCommitment?.Task?.Message ?? string.Empty;

            return new CommitmentViewModel
            {
                Commitment = data.Commitment,
                LatestMessage = message,
                PendingChanges = PendingChanges(data.Commitment?.Apprenticeships)
            };
        }

        public async Task<ExtendedApprenticeshipViewModel> GetApprenticeship(long providerId, long commitmentId, long apprenticeshipId)
        {
            var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
            {
                ProviderId = providerId,
                CommitmentId = commitmentId,
                AppenticeshipId = apprenticeshipId
            });

            var standards = await _mediator.SendAsync(new GetStandardsQueryRequest());
            
            var apprenticeship = MapFrom(data.Apprenticeship);

            apprenticeship.ProviderId = providerId;

            return new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = apprenticeship,
                ApprenticeshipProgrammes = await GetTrainingProgrammes()
            };
        }

        public async Task<ExtendedApprenticeshipViewModel> GetApprenticeship(long providerId, long commitmentId)
        {
            var standards = await _mediator.SendAsync(new GetStandardsQueryRequest());

            var apprenticeship = new ApprenticeshipViewModel
            {
                ProviderId = providerId,
                CommitmentId = commitmentId,
            };

            return new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = apprenticeship,
                ApprenticeshipProgrammes = await GetTrainingProgrammes()
            };
        }

        public async Task UpdateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            await _mediator.SendAsync(new UpdateApprenticeshipCommand
            {
                ProviderId = apprenticeship.ProviderId,
                Apprenticeship = await MapFrom(apprenticeship)
            });
        }

        public async Task CreateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            await _mediator.SendAsync(new CreateApprenticeshipCommand
            {
                ProviderId = apprenticeship.ProviderId,
                Apprenticeship = await MapFrom(apprenticeship)
            });
        }

        public async Task SubmitCommitment(SubmitCommitmentViewModel model)
        {
            await _mediator.SendAsync(new SubmitCommitmentCommand
            {
                ProviderId = model.ProviderId,
                CommitmentId = model.CommitmentId,
                Message = model.Message,
                SaveOrSend = model.SaveOrSend
            });
        }

        public async Task ApproveCommitment(long providerId, long commitmentId, string saveOrSend)
        {
            await _mediator.SendAsync(new SubmitCommitmentCommand
            {
                ProviderId = providerId,
                CommitmentId = commitmentId,
                Message = string.Empty,
                SaveOrSend = saveOrSend
            });
        }

        public async Task<FinishEditingViewModel> GetFinishEditing(long providerId, long commitmentId)
        {
            var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                ProviderId = providerId,
                CommitmentId = commitmentId
            });

            bool approveAndSend = PendingChanges(data.Commitment?.Apprenticeships);

            return new FinishEditingViewModel
            {
                CommitmentId = commitmentId,
                ProviderId = providerId,
                ApproveAndSend = approveAndSend
            };
        }

        private static bool PendingChanges(List<Apprenticeship> apprenticeships)
        {
            if (apprenticeships == null || !apprenticeships.Any()) return true;
            return apprenticeships?.Any(m => m.AgreementStatus == AgreementStatus.NotAgreed
                                   || m.AgreementStatus == AgreementStatus.ProviderAgreed) ?? false;
        }

        private ApprenticeshipViewModel MapFrom(Apprenticeship apprenticeship)
        {
            var dateOfBirth = apprenticeship.DateOfBirth;
            return new ApprenticeshipViewModel
            {
                Id = apprenticeship.Id,
                CommitmentId = apprenticeship.CommitmentId,
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                DateOfBirthDay = dateOfBirth?.Day,
                DateOfBirthMonth = dateOfBirth?.Month,
                DateOfBirthYear = dateOfBirth?.Year,
                NINumber = apprenticeship.NINumber,
                ULN = apprenticeship.ULN,
                TrainingType = apprenticeship.TrainingType,
                TrainingCode = apprenticeship.TrainingCode,
                Cost = NullableDecimalToString(apprenticeship.Cost),
                StartMonth = apprenticeship.StartDate?.Month, 
                StartYear = apprenticeship.StartDate?.Year,
                EndMonth = apprenticeship.EndDate?.Month,
                EndYear = apprenticeship.EndDate?.Year,
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

        private async Task<Apprenticeship> MapFrom(ApprenticeshipViewModel viewModel)
        {

            var apprenticeship =  new Apprenticeship
            {
                Id = viewModel.Id,
                CommitmentId = viewModel.CommitmentId,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                DateOfBirth = GetDateTime(viewModel.DateOfBirthDay, viewModel.DateOfBirthMonth, viewModel.DateOfBirthYear),
                NINumber = viewModel.NINumber,
                ULN = viewModel.ULN,
                Cost = viewModel.Cost == null ? default(decimal?) : decimal.Parse(viewModel.Cost),
                StartDate = GetDateTime(viewModel.StartMonth, viewModel.StartYear),
                EndDate = GetDateTime(viewModel.EndMonth, viewModel.EndYear),
                ProviderRef = viewModel.ProviderRef,
                EmployerRef = viewModel.EmployerRef
            };

            if (!string.IsNullOrWhiteSpace(viewModel.TrainingCode))
            {
                var training = await GetTrainingProgramme(viewModel.TrainingCode);
                apprenticeship.TrainingType = training is Standard ? Commitments.Api.Types.TrainingType.Standard : Commitments.Api.Types.TrainingType.Framework;
                apprenticeship.TrainingCode = viewModel.TrainingCode;
                apprenticeship.TrainingName = training.Title;
            }

            return apprenticeship;
        }

        private DateTime? GetDateTime(int? month, int? year)
        {
            if (month.HasValue && year.HasValue)
                return new DateTime(year.Value, month.Value, 1);

            return null;
        }

        private DateTime? GetDateTime(int? day, int? month, int? year)
        {
            if (day.HasValue && month.HasValue && year.HasValue)
            {
                DateTime dateOfBirthOut;
                if (DateTime.TryParseExact(
                    $"{year.Value}-{month.Value}-{day.Value}",
                    "yyyy-M-d",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirthOut))
                {
                    return dateOfBirthOut;
                }
            }

            return null;
        }

        private async Task<ITrainingProgramme> GetTrainingProgramme(string trainingCode)
        {
            var id = int.Parse(trainingCode);

            return (await GetTrainingProgrammes()).Single(x => x.Id == id);
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
    }
}