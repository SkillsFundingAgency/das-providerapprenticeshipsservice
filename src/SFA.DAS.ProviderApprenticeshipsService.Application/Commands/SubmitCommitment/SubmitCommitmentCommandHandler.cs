using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.Tasks.Api.Client;
using SFA.DAS.Tasks.Api.Types.Templates;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment
{
    public class SubmitCommitmentCommandHandler : AsyncRequestHandler<SubmitCommitmentCommand>
    {
        private readonly ICommitmentsApi _commitmentsApi;
        private readonly ITasksApi _tasksApi;
        private readonly AbstractValidator<SubmitCommitmentCommand> _validator;
        private readonly IMediator _mediator;
        private readonly ProviderApprenticeshipsServiceConfiguration _configuration;

        public SubmitCommitmentCommandHandler(ICommitmentsApi commitmentsApi, ITasksApi tasksApi, AbstractValidator<SubmitCommitmentCommand> validator, IMediator mediator, ProviderApprenticeshipsServiceConfiguration configuration)
        {
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));
            if (tasksApi == null)
                throw new ArgumentNullException(nameof(tasksApi));
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _commitmentsApi = commitmentsApi;
            _tasksApi = tasksApi;
            _validator = validator;
            _mediator = mediator;
            _configuration = configuration;
        }

        protected override async Task HandleCore(SubmitCommitmentCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid)
                throw new InvalidRequestException(validationResult.Errors);

            var commitment = await _commitmentsApi.GetProviderCommitment(message.ProviderId, message.CommitmentId);

            if (commitment.ProviderId != message.ProviderId)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Commitment", "This commitment does not belong to this Provider" } });

            var submission = new CommitmentSubmission { Action = message.LastAction, LastUpdatedByInfo = 
                new LastUpdateInfo
                    {
                        Name = message?.UserDisplayName ?? "",
                        EmailAddress = message?.UserEmailAddress ?? ""
                    },
                    UserId = message.UserId
                };

            await _commitmentsApi.PatchProviderCommitment(message.ProviderId, message.CommitmentId, submission);

            if (message.CreateTask)
                await CreateTask(message, commitment);

            if (_configuration.EnableEmailNotifications && message.LastAction != LastAction.None)
            {
                var notificationCommand = BuildNotificationCommand(commitment, message.LastAction,
                    message.HashedCommitmentId);

                await _mediator.SendAsync(notificationCommand);
            }
        }

        private SendNotificationCommand BuildNotificationCommand(Commitment commitment, LastAction action, string hashedCommitmentId)
        {
            return new SendNotificationCommand
            {
                Email = new Email
                {
                    RecipientsAddress = commitment.EmployerLastUpdateInfo.EmailAddress,
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Subject = "<Test Employer Notification>", // Replaced by Notify Service
                    SystemId = "x", // Don't need to populate
                    TemplateId = "EmployerCommitmentNotification",
                    Tokens = new Dictionary<string, string>
                    {
                        { "type", action == LastAction.Approve ? "approval" : "review" },
                        { "cohort_reference", hashedCommitmentId },
                    }
                }
            };
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

                await _tasksApi.CreateTask(task.Assignee, task);
            }
        }
    }
}