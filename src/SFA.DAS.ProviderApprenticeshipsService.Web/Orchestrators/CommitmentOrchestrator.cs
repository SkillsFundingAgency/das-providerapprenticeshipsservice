using System;
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
using System.Globalization;
using NLog;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class CommitmentOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ICommitmentStatusCalculator _statusCalculator;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public CommitmentOrchestrator(IMediator mediator, ICommitmentStatusCalculator statusCalculator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (statusCalculator == null)
                throw new ArgumentNullException(nameof(statusCalculator));

            _mediator = mediator;
            _statusCalculator = statusCalculator;
        }

        public async Task<CommitmentListViewModel> GetAll(long providerId)
        {
            Logger.Info($"Getting all commitments for provider:{providerId}");

            var data = await _mediator.SendAsync(new GetCommitmentsQueryRequest
            {
                ProviderId = providerId
            });

            return new CommitmentListViewModel
            {
                ProviderId = providerId,
                Commitments = MapFrom(data.Commitments)
            };
        }

        public async Task<CommitmentDetailsViewModel> GetCommitmentDetails(long providerId, long commitmentId)
        {
            Logger.Info($"Getting commitment details:{commitmentId} for provider:{providerId}");

            var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                ProviderId = providerId,
                CommitmentId = commitmentId
            });

            string message = await GetLatestMessage(providerId, commitmentId);

            return new CommitmentDetailsViewModel
            {
                ProviderId = providerId,
                CommitmentId = data.Commitment.Id,
                LegalEntityName = data.Commitment.LegalEntityName,
                Reference = data.Commitment.Reference,
                Status = _statusCalculator.GetStatus(data.Commitment.EditStatus, data.Commitment.Apprenticeships.Count, data.Commitment.AgreementStatus),
                Apprenticeships = MapFrom(data.Commitment.Apprenticeships),
                LatestMessage = message,
                PendingChanges = data.Commitment.AgreementStatus != AgreementStatus.EmployerAgreed
            };
        }

        public async Task<CommitmentListItemViewModel> GetCommitment(long providerId, long commitmentId)
        {
            Logger.Info($"Getting commitment:{commitmentId} for provider:{providerId}");

            var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                ProviderId = providerId,
                CommitmentId = commitmentId
            });

            return MapFrom(data.Commitment);
        }

        public async Task<ExtendedApprenticeshipViewModel> GetApprenticeship(long providerId, long commitmentId, long apprenticeshipId)
        {
            Logger.Info($"Getting apprenticeship:{apprenticeshipId} for provider:{providerId}");

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

        public async Task<ExtendedApprenticeshipViewModel> GetCreateApprenticeshipViewModel(long providerId, long commitmentId)
        {
            Logger.Info($"Getting info for creating apprenticeship for provider:{providerId} commitment:{commitmentId}");

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

        public async Task CreateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            Logger.Info($"Creating apprenticeship for provider:{apprenticeship.ProviderId} commitment:{apprenticeship.CommitmentId}");

            await _mediator.SendAsync(new CreateApprenticeshipCommand
            {
                ProviderId = apprenticeship.ProviderId,
                Apprenticeship = await MapFrom(apprenticeship)
            });
        }

        public async Task UpdateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            Logger.Info($"Updating apprenticeship for provider:{apprenticeship.ProviderId} commitment:{apprenticeship.CommitmentId}");

            await _mediator.SendAsync(new UpdateApprenticeshipCommand
            {
                ProviderId = apprenticeship.ProviderId,
                Apprenticeship = await MapFrom(apprenticeship)
            });
        }


        public async Task SubmitCommitment(SubmitCommitmentViewModel model)
        {
            Logger.Info($"Submitting ({model.SaveOrSendStatus}) Commitment for provider:{model.ProviderId} commitment:{model.CommitmentId}");

            // ToDo: Unit tests
            // ToDo: Merge with ApproveCommitment method?
            var agreementStatus = model.SaveOrSendStatus != SaveOrSendStatus.Save
                                ? AgreementStatus.ProviderAgreed
                                : AgreementStatus.NotAgreed;
            await _mediator.SendAsync(new SubmitCommitmentCommand
            {
                ProviderId = model.ProviderId,
                CommitmentId = model.CommitmentId,
                Message = model.Message,
                AgreementStatus = agreementStatus,
                CreateTask = model.SaveOrSendStatus != SaveOrSendStatus.Approve
        });
        }

        public async Task ApproveCommitment(long providerId, long commitmentId, SaveOrSendStatus saveOrSendStatus)
        {
            Logger.Info($"Approving ({saveOrSendStatus}) Commitment for provider:{providerId} commitment:{commitmentId}");

            // ToDo: Unit tests
            var agreementStatus = saveOrSendStatus != SaveOrSendStatus.Save
                                ? AgreementStatus.ProviderAgreed
                                : AgreementStatus.NotAgreed;

            await _mediator.SendAsync(new SubmitCommitmentCommand
            {
                ProviderId = providerId,
                CommitmentId = commitmentId,
                Message = string.Empty,
                AgreementStatus = agreementStatus,
                CreateTask = saveOrSendStatus != SaveOrSendStatus.Approve
        });
        }

        public async Task<FinishEditingViewModel> GetFinishEditing(long providerId, long commitmentId)
        {
            Logger.Info($"Get info for finish editing options for provider:{providerId} commitment:{commitmentId}");

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

        private IList<ApprenticeshipListItemViewModel> MapFrom(List<Apprenticeship> apprenticeships)
        {
            var apprenticeViewModels = apprenticeships.Select(x => new ApprenticeshipListItemViewModel
            {
                ApprenticeshipId = x.Id,
                ApprenticeshipName = x.ApprenticeshipName,
                ULN = x.ULN,
                TrainingName = x.TrainingName,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Cost = x.Cost
            }).ToList();

            return apprenticeViewModels;
        }

        private async Task<string> GetLatestMessage(long providerId, long commitmentId)
        {
            var allTasks = await _mediator.SendAsync(new GetTasksQueryRequest { ProviderId = providerId });

            var taskForCommitment = allTasks?.Tasks
                .Select(x => new { Task = JsonConvert.DeserializeObject<CreateCommitmentTemplate>(x.Body), CreateDate = x.CreatedOn })
                .Where(x => x.Task != null && x.Task.CommitmentId == commitmentId)
                .OrderByDescending(x => x.CreateDate)
                .FirstOrDefault();

            var message = taskForCommitment?.Task?.Message ?? string.Empty;

            return message;
        }

        private static bool PendingChanges(List<Apprenticeship> apprenticeships)
        {
            if (apprenticeships == null || !apprenticeships.Any()) return true;
            return apprenticeships?.Any(m => m.AgreementStatus == AgreementStatus.NotAgreed
                                   || m.AgreementStatus == AgreementStatus.ProviderAgreed) ?? false;
        }

        // TODO: Move mappers into own class
        private List<CommitmentListItemViewModel> MapFrom(List<CommitmentListItem> commitments)
        {
            var commitmentsList = commitments.Select(x => MapFrom(x)).ToList();

            return commitmentsList;
        }

        private CommitmentListItemViewModel MapFrom(CommitmentListItem listItem)
        {
            return new CommitmentListItemViewModel
            {
                CommitmentId = listItem.Id,
                Reference = listItem.Reference,
                LegalEntityName = listItem.LegalEntityName,
                ProviderName = listItem.ProviderName,
                Status = _statusCalculator.GetStatus(listItem.EditStatus, listItem.ApprenticeshipCount, listItem.AgreementStatus),
                ShowViewLink = listItem.EditStatus == EditStatus.ProviderOnly
            };
        }

        private CommitmentListItemViewModel MapFrom(Commitment listItem)
        {
            return new CommitmentListItemViewModel
            {
                CommitmentId = listItem.Id,
                Reference = listItem.Reference,
                LegalEntityName = listItem.LegalEntityName,
                ProviderName = listItem.ProviderName,
                Status = _statusCalculator.GetStatus(listItem.EditStatus, listItem.Apprenticeships.Count, listItem.AgreementStatus),
                ShowViewLink = listItem.EditStatus == EditStatus.ProviderOnly
            };
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
    }
}