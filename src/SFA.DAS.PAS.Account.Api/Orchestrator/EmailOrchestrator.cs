using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.PAS.Account.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.PAS.Account.Api.Orchestrator;

public interface IEmailOrchestrator
{
    Task SendEmailToAllProviderRecipients(long providerId, ProviderEmailRequest providerEmailRequest);
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

    public async Task SendEmailToAllProviderRecipients(long providerId, ProviderEmailRequest providerEmailRequest)
    {
        List<string> recipients;

        var accountUsers = (await _accountOrchestrator.GetAccountUsers(providerId)).ToList();

        if (providerEmailRequest.ExplicitEmailAddresses != null && providerEmailRequest.ExplicitEmailAddresses.Any())
        {
            _logger.LogInformation("Explicit recipients requested for email");

            recipients = providerEmailRequest.ExplicitEmailAddresses.ToList();
        }
        else
        {
            recipients = accountUsers.Any(user => !user.IsSuperUser)
                ? accountUsers.Where(user => !user.IsSuperUser).Select(x => x.EmailAddress).ToList()
                : accountUsers.Select(user => user.EmailAddress).ToList();
        }

        var optedOutList = accountUsers.Where(user => !user.ReceiveNotifications).Select(x => x.EmailAddress).ToList();

        var finalRecipients = recipients.Where(recipient =>
                !optedOutList.Any(y => recipient.Equals(y, StringComparison.CurrentCultureIgnoreCase)))
            .ToList();

        var commands = finalRecipients.Select(recipient => new SendNotificationCommand(CreateEmailForRecipient(recipient, providerEmailRequest)));

        await Task.WhenAll(commands.Select(command => _mediator.Send(command)));

        _logger.LogInformation("Sent email to {FinalRecipientsCount} recipients for ukprn: {ProviderId}", finalRecipients.Count, providerId);
    }

    private static NotificationEmail CreateEmailForRecipient(string recipient, ProviderEmailRequest source)
    {
        return new NotificationEmail(source.TemplateId, recipient, new Dictionary<string, string>(source.Tokens));
    }
}