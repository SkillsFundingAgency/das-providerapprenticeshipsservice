using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

namespace SFA.DAS.PAS.Account.Api.Orchestrator
{
    public class EmailOrchestrator
    {
        private readonly IAccountOrchestrator _accountOrchestrator;
        private readonly IMediator _mediator;
        private readonly IIdamsEmailServiceWrapper _idamsEmailServiceWrapper;

        public EmailOrchestrator(IAccountOrchestrator accountOrchestrator, IMediator mediator, IIdamsEmailServiceWrapper idamsEmailServiceWrapper)
        {
            _accountOrchestrator = accountOrchestrator;
            _mediator = mediator;
            _idamsEmailServiceWrapper = idamsEmailServiceWrapper;
        }

        public async Task SendEmailToAllProviderRecipients(long ukprn, ProviderEmailRequest message)
        {
            List<string> recipients = new List<string>();
            if (message.ExplicitEmailAddresses != null)
            {
                recipients = message.ExplicitEmailAddresses.ToList();
            }

            if (!recipients.Any())
            {
                recipients = await GetIdamsRecipients(ukprn);
            }

            if(!recipients.Any())
            {
                var accountUsers = await _accountOrchestrator.GetAccountUsers(ukprn);
                recipients = accountUsers.Select(x => x.EmailAddress).ToList();
            }

            //todo log warning no recipients + logging in generalement

            var commands = recipients.Select(x => new SendNotificationCommand{ Email = CreateEmailForRecipient(x, message) });
            await Task.WhenAll(commands.Select(x => _mediator.Send(x)));
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
                ReplyToAddress = "noreply@sfa.gov.uk", //todo do we want to set this in configuration?
                Subject = "x",
                SystemId = "x"
            };
        }
    }
}