using System;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Tasks.Api.Client;
using SFA.DAS.Tasks.Api.Types.Templates;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment
{
    public class SubmitCommitmentCommandHandler : AsyncRequestHandler<SubmitCommitmentCommand>
    {
        private readonly ICommitmentsApi _commitmentsApi;
        private readonly ITasksApi _tasksApi;
        private readonly SubmitCommitmentCommandValidator _validator;

        public SubmitCommitmentCommandHandler(ICommitmentsApi commitmentsApi, ITasksApi tasksApi)
        {
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));
            if (tasksApi == null)
                throw new ArgumentNullException(nameof(tasksApi));

            _commitmentsApi = commitmentsApi;
            _tasksApi = tasksApi;
            _validator = new SubmitCommitmentCommandValidator();
        }

        protected override async Task HandleCore(SubmitCommitmentCommand message)
        {
             var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid)
                throw new InvalidRequestException(validationResult.Errors);

            var commitment = await _commitmentsApi.GetProviderCommitment(message.ProviderId, message.CommitmentId);

            var agreementStatus = AgreementStatus.NotAgreed;
            // TODO: Refactor out these magic strings
            if (message.SaveOrSend != "save-no-send")
            {
                agreementStatus = AgreementStatus.ProviderAgreed;
            }

            await _commitmentsApi.PatchProviderCommitment(message.ProviderId, message.CommitmentId, agreementStatus);

            // TODO: Refactor out these magic strings
            if (message.SaveOrSend != "approve")
                await CreateTask(message, commitment);
        }

        private async Task CreateTask(SubmitCommitmentCommand message, Commitment commitment)
        {
            if (!string.IsNullOrWhiteSpace(message.Message))
            {
                var taskTemplate = new SubmitCommitmentTemplate
                    {
                        CommitmentId = message.CommitmentId,
                        Message = message.Message,
                        Source =
                            $"PROVIDER-{message.ProviderId}-{commitment.ProviderName}"
                    };

                var task = new Tasks.Api.Types.Task
                    {
                        Assignee = $"EMPLOYER-{commitment.EmployerAccountId}",
                        TaskTemplateId = SubmitCommitmentTemplate.TemplateId,
                        Name = $"Submit Commitment - {commitment.Reference}",
                        Body = JsonConvert.SerializeObject(taskTemplate)
                    };

                await this._tasksApi.CreateTask(task.Assignee, task);
            }
        }
    }
}