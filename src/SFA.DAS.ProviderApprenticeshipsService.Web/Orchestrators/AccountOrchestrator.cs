using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UnsubscribeNotification;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderHasRelationshipWithPermission;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUser;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Settings;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class AccountOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public AccountOrchestrator(IMediator mediator,
            ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<AccountHomeViewModel> GetAccountHomeViewModel(int providerId)
        {
            try
            {
                _logger.Info($"Getting provider {providerId}");

                var providerResponse = await _mediator.Send(new GetProviderQueryRequest { UKPRN = providerId });

                return new AccountHomeViewModel
                {
                    AccountStatus = AccountStatus.Active,
                    ProviderName = providerResponse.ProvidersView.Provider.ProviderName,
                    ProviderId = providerId,
                    ShowAcademicYearBanner = false,
                    ShowTraineeshipLink = _featureToggleService.Get<Traineeships>().FeatureEnabled
                };
            }
            catch (Exception ex)
            {
                _logger.Warn(ex,$"Provider {providerId} details not found in provider information service");

                return new AccountHomeViewModel { AccountStatus = AccountStatus.NoAgreement };
            }
        }

        public async Task<NotificationSettingsViewModel> GetNotificationSettings(string userRef)
        {
            _logger.Info($"Getting setting for user {userRef}");

            var response = await _mediator.Send(new GetUserNotificationSettingsQuery
            {
                UserRef = userRef
            });

            var model = new NotificationSettingsViewModel
            {
                NotificationSettings = Map(response.NotificationSettings)
            };

            _logger.Trace($"Found {response.NotificationSettings.Count} settings for user {userRef}");

            return model;
        }

        public async Task UpdateNotificationSettings(NotificationSettingsViewModel model)
        {
            var setting = model.NotificationSettings.First();
            _logger.Info($"Uppdating setting for user {setting.UserRef}");

            await _mediator.Send(new UpdateUserNotificationSettingsCommand
            {
                UserRef = setting.UserRef,
                ReceiveNotifications = setting.ReceiveNotifications
            });

            _logger.Trace($"Updated receive notification to {setting.ReceiveNotifications} for user {setting.UserRef}");
        }

        public async Task<SummaryUnsubscribeViewModel> Unsubscribe(string userRef, string urlSettingsPage)
        {
            var userSettings = await _mediator.Send(new GetUserNotificationSettingsQuery { UserRef = userRef });
            var user = await _mediator.Send(new GetUserQuery { UserRef = userRef });

            var alreadyUnsubscribed = !userSettings.NotificationSettings.FirstOrDefault()?.ReceiveNotifications == true;
            if (userSettings.NotificationSettings.FirstOrDefault()?.ReceiveNotifications == true)
            {
                await _mediator.Send(new UnsubscribeNotificationRequest { UserRef = userRef });
                await _mediator.Send(BuildNotificationCommand(user.EmailAddress, user.Name, urlSettingsPage));
            }

            return new SummaryUnsubscribeViewModel { AlreadyUnsubscribed = alreadyUnsubscribed };
        }

        private IList<UserNotificationSetting> Map(
       IEnumerable<Domain.Models.Settings.UserNotificationSetting> notificationSettings)
        {
            if (notificationSettings == null) return new List<UserNotificationSetting>(0);

            return
                notificationSettings.Select(m =>
                   new UserNotificationSetting { UserRef = m.UserRef, ReceiveNotifications = m.ReceiveNotifications })
                    .ToList();
        }

        private SendNotificationCommand BuildNotificationCommand(string emailAddress, string userDisplayName, string urlToSettingsPage)
        {
            return new SendNotificationCommand
            {
                Email = new Email
                {
                    RecipientsAddress = emailAddress,
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Subject = "<Test Employer Notification>", // Replaced by Notify Service
                    SystemId = "x", // Don't need to populate
                    TemplateId = "ProviderUnsubscribeAlertSummaryNotification",
                    Tokens = new Dictionary<string, string>
                    {
                        { "name", userDisplayName },
                        { "link_to_notify_settings", urlToSettingsPage }
                    }
                }
            };
        }

        public async Task<bool> ProviderHasPermission(long providerId, Operation permission)
        {
            var permissionResponse = await _mediator.Send(new GetProviderHasRelationshipWithPermissionQueryRequest
            {
                Permission = permission,
                ProviderId = providerId
            });

            return permissionResponse.HasPermission;
        }
    }
}