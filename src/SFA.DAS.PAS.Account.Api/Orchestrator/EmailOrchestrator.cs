using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Owin.Security.Provider;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteRegisteredUser;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

namespace SFA.DAS.PAS.Account.Api.Orchestrator
{
    public class EmailOrchestrator
    {
        private readonly IAccountOrchestrator _accountOrchestrator;
        private readonly IMediator _mediator;
        private readonly IIdamsEmailServiceWrapper _idamsEmailServiceWrapper;
        private readonly IProviderCommitmentsLogger _logger;

        public EmailOrchestrator(IAccountOrchestrator accountOrchestrator, IMediator mediator, IIdamsEmailServiceWrapper idamsEmailServiceWrapper, IProviderCommitmentsLogger logger)
        {
            _accountOrchestrator = accountOrchestrator;
            _mediator = mediator;
            _idamsEmailServiceWrapper = idamsEmailServiceWrapper;
            _logger = logger;
        }

        public async Task SendEmailToAllProviderRecipients(long providerId, ProviderEmailRequest message)
        {
            var recipients = new List<string>();

            _logger.Info($"Retrieving DAS Users and Super Users from Provider IDAMS for Provider {providerId}");
            var idamsUsersTask = _idamsEmailServiceWrapper.GetEmailsAsync(providerId);
            var idamsSuperUsersTask = _idamsEmailServiceWrapper.GetSuperUserEmailsAsync(providerId);
            var idamsError = false;
            try
            {
                await Task.WhenAll(idamsUsersTask, idamsSuperUsersTask);
            }
            catch (Exception ex)
            {
                idamsError = true;
                _logger.Error(ex, "An error occurred retrieving users from Provider IDAMS");
                idamsUsersTask = Task.FromResult(new List<string>());
                idamsSuperUsersTask = Task.FromResult(new List<string>());
            }

            var idamsUsers = await idamsUsersTask;
            var idamsSuperUsers = await idamsSuperUsersTask;
            var allIdamsUsers = idamsUsers.Concat(idamsSuperUsers).Distinct().ToList();
            _logger.Info($"{allIdamsUsers.Count} total users retrieved from IDAMS for Provider {providerId} ({idamsUsers.Count} DAS Users; {idamsSuperUsers.Count} Super Users)");

            var accountUsers = (await _accountOrchestrator.GetAccountUsers(providerId)).ToList();

            if (!idamsError) {  await RemoveAccountUsersNotInIdams(accountUsers, allIdamsUsers); }

            if (!recipients.Any() && message.ExplicitEmailAddresses != null)
            {
                _logger.Info("Explicit recipients requested for email");

                recipients = message.ExplicitEmailAddresses.ToList();

                if (idamsError)
                {
                    _logger.Info("Absence from IDAMS cannot be ascertained so presence is assumed - email message will not be suppressed");
                    return;
                }

                recipients.RemoveAll(x => !allIdamsUsers.Contains(x));
                if (!recipients.Any())
                {
                    _logger.Warn("All recipients explicitly requested for email are absent from Provider IDAMS - email message will be suppressed");
                    return;
                }
            }

            if (!recipients.Any())
            {
                recipients = idamsUsers.Any() ? idamsUsers : idamsSuperUsers;
            }

            if (!recipients.Any())
            {
                recipients = accountUsers.Select(x=>x.EmailAddress).ToList();
            }

            var optedOutList = accountUsers.Where(x => !x.ReceiveNotifications).Select(x => x.EmailAddress).ToList();

            var finalRecipients = recipients.Where(x =>
                !optedOutList.Any(y => x.Equals(y, StringComparison.CurrentCultureIgnoreCase)))
                .ToList();

            var commands = finalRecipients
                .Select(x => new SendNotificationCommand{ Email = CreateEmailForRecipient(x, message) });
            await Task.WhenAll(commands.Select(x => _mediator.Send(x)));

            _logger.Info($"Sent email to {finalRecipients.Count} recipients for ukprn: {providerId}", providerId);
        }

        private async Task RemoveAccountUsersNotInIdams(List<User> accountUsers, List<string> allIdamsUsers)
        {
            var removedUsers = new List<User>();

            foreach (var accountUser in accountUsers)
            {
                if (!allIdamsUsers.Contains(accountUser.EmailAddress, StringComparer.CurrentCultureIgnoreCase))
                {
                    removedUsers.Add(accountUser);
                }
            }

            foreach (var user in removedUsers)
            {
                _logger.Info($"User {user.UserRef} not found IDAMS will be marked as deleted");
                await _mediator.Send(new DeleteRegisteredUserCommand { UserRef = user.UserRef });
                accountUsers.Remove(user);
            }
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