﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

namespace SFA.DAS.PAS.Account.Api.Orchestrator
{
    public class EmailOrchestrator
    {
        private readonly IAccountOrchestrator _accountOrchestrator;
        private readonly IMediator _mediator;
        private readonly IIdamsEmailServiceWrapper _idamsEmailServiceWrapper;
        private readonly ProviderApprenticeshipsServiceConfiguration _configuration;

        public EmailOrchestrator(IAccountOrchestrator accountOrchestrator, IMediator mediator, IIdamsEmailServiceWrapper idamsEmailServiceWrapper, ProviderApprenticeshipsServiceConfiguration configuration)
        {
            _accountOrchestrator = accountOrchestrator;
            _mediator = mediator;
            _idamsEmailServiceWrapper = idamsEmailServiceWrapper;
            _configuration = configuration;
        }

        public async Task SendEmailToAllProviderRecipients(long ukprn, ProviderEmailRequest message)
        {
            List<string> recipients = new List<string>();

            if (!_configuration.CommitmentNotification.UseProviderEmail)
            {
                recipients = _configuration.CommitmentNotification.ProviderTestEmails;
            }

            if (!recipients.Any() && message.ExplicitEmailAddresses != null)
            {
                recipients = message.ExplicitEmailAddresses.ToList();
            }

            if (!recipients.Any())
            {
                recipients = await GetIdamsRecipients(ukprn);
            }

            if(!recipients.Any())
            {
                recipients = await GetAccountUserRecipients(ukprn);
            }

            var commands = recipients.Select(x => new SendNotificationCommand{ Email = CreateEmailForRecipient(x, message) });
            await Task.WhenAll(commands.Select(x => _mediator.Send(x)));
        }

        private async Task<List<string>> GetAccountUserRecipients(long ukprn)
        {
            var accountUsers = await _accountOrchestrator.GetAccountUsers(ukprn);
            return accountUsers.Where(x => x.ReceiveNotifications).Select(x => x.EmailAddress).ToList();
        }

        private async Task<List<string>> GetIdamsRecipients(long ukprn)
        {
            var recipients = await _idamsEmailServiceWrapper.GetEmailsAsync(ukprn);
            if (recipients.Any())
            {
                return recipients;
            }

            recipients = await _idamsEmailServiceWrapper.GetSuperUserEmailsAsync(ukprn);
            if (recipients.Any())
            {
                return recipients;
            }

            return new List<string>();
        }

        private Email CreateEmailForRecipient(string recipient, ProviderEmailRequest source)
        {
            return new Email
            {
                RecipientsAddress = recipient,
                TemplateId = source.TemplateId,
                Tokens = new Dictionary<string, string>(source.Tokens),
                ReplyToAddress = "noreply@sfa.gov.uk",
                Subject = "x",
                SystemId = "x"
            };
        }
    }
}