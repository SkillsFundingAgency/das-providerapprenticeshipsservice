using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment
{
    public class SubmitCommitmentCommandHandler : AsyncRequestHandler<SubmitCommitmentCommand>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;
        private readonly AbstractValidator<SubmitCommitmentCommand> _validator;
        private readonly IMediator _mediator;
        private readonly ProviderApprenticeshipsServiceConfiguration _configuration;

        public SubmitCommitmentCommandHandler(IProviderCommitmentsApi commitmentsApi, AbstractValidator<SubmitCommitmentCommand> validator, IMediator mediator, ProviderApprenticeshipsServiceConfiguration configuration)
        {
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _commitmentsApi = commitmentsApi;
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
                    Message = message.Message,
                    UserId = message.UserId
                };

            await _commitmentsApi.PatchProviderCommitment(message.ProviderId, message.CommitmentId, submission);

            if (_configuration.EnableEmailNotifications && message.LastAction != LastAction.None)
            {
                await SendEmailNotification(message, commitment);
            }
        }

        private async Task SendEmailNotification(SubmitCommitmentCommand message, CommitmentView commitment)
        {
            var notificationCommand = BuildNotificationCommand(commitment, message.LastAction,
                                message.HashedCommitmentId, message.UserDisplayName);

            await _mediator.SendAsync(notificationCommand);
        }

        private SendNotificationCommand BuildNotificationCommand(CommitmentView commitment, LastAction action, string hashedCommitmentId, string displayName)
        {
            var template = commitment.AgreementStatus == AgreementStatus.NotAgreed ? "EmployerCommitmentNotification" : "EmployerCohortApproved";

            return new SendNotificationCommand
            {
                Email = new Email
                {
                    RecipientsAddress = commitment.EmployerLastUpdateInfo.EmailAddress,
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Subject = "<Test Employer Notification>", // Replaced by Notify Service
                    SystemId = "x", // Don't need to populate
                    TemplateId = template,
                    Tokens = new Dictionary<string, string>
                    {
                        { "type", action == LastAction.Approve ? "approval" : "review" },
                        { "cohort_reference", hashedCommitmentId },
                        { "first_name", displayName },
                    }
                }
            };
        }
    }
}