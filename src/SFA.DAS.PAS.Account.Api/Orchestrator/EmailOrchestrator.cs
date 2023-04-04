using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.PAS.Account.Application.Commands.SendNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.PAS.Account.Api.Orchestrator;

public interface IEmailOrchestrator
{
    Task SendEmailToAllProviderRecipients(long providerId, ProviderEmailRequest message);
}

public class EmailOrchestrator : IEmailOrchestrator
{
    private readonly IAccountOrchestrator _accountOrchestrator;
    private readonly IMediator _mediator;
    private readonly ILogger<EmailOrchestrator> _logger;

    public EmailOrchestrator(IAccountOrchestrator accountOrchestrator, IMediator mediator, ILogger<EmailOrchestrator> logger)
    {
        _accountOrchestrator = accountOrchestrator;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task SendEmailToAllProviderRecipients(long providerId, ProviderEmailRequest message)
    {
        List<string> recipients;

        var accountUsers = (await _accountOrchestrator.GetAccountUsers(providerId)).ToList();

        if (message.ExplicitEmailAddresses != null && message.ExplicitEmailAddresses.Any())
        {
            _logger.LogInformation("Explicit recipients requested for email");

            recipients = message.ExplicitEmailAddresses.ToList();
        }
        else
        {
            recipients = accountUsers.Any(u => !u.IsSuperUser) ? accountUsers.Where(x => !x.IsSuperUser).Select(x => x.EmailAddress).ToList() 
                : accountUsers.Select(x => x.EmailAddress).ToList();
        }

        var optedOutList = accountUsers.Where(x => !x.ReceiveNotifications).Select(x => x.EmailAddress).ToList();

        var finalRecipients = recipients.Where(x =>
                !optedOutList.Any(y => x.Equals(y, StringComparison.CurrentCultureIgnoreCase)))
            .ToList();

        var commands = finalRecipients.Select(x => new SendNotificationCommand{ Email = CreateEmailForRecipient(x, message) });
        await Task.WhenAll(commands.Select(x => _mediator.Send(x)));

        _logger.LogInformation($"Sent email to {finalRecipients.Count} recipients for ukprn: {providerId}", providerId);
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