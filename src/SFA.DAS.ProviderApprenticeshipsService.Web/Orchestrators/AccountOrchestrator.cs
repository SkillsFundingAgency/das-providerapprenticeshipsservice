using SFA.DAS.Authorization.Services;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UnsubscribeNotification;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUser;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Features;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Settings;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

public interface IAccountOrchestrator
{
    Task<AccountHomeViewModel> GetAccountHomeViewModel(int providerId);
    Task<NotificationSettingsViewModel> GetNotificationSettings(string userRef, string email);
    Task UpdateNotificationSettings(NotificationSettingsViewModel model);
    Task<SummaryUnsubscribeViewModel> Unsubscribe(string userRef, string urlSettingsPage);
}

public class AccountOrchestrator : IAccountOrchestrator
{
    private readonly IMediator _mediator;
    private readonly ILogger<AccountOrchestrator> _logger;
    private readonly IAuthorizationService _authorizationService;
    private readonly IHtmlHelpers _htmlHelpers;
    private readonly ICurrentDateTime _currentDateTime;
    private readonly ProviderApprenticeshipsServiceConfiguration _configuration;

    public AccountOrchestrator(
        IMediator mediator,
        ILogger<AccountOrchestrator> logger,
        IAuthorizationService authorizationService,
        IHtmlHelpers htmlHelpers, 
        ICurrentDateTime currentDateTime,
        ProviderApprenticeshipsServiceConfiguration configuration)
    {
        _mediator = mediator;
        _logger = logger;
        _authorizationService = authorizationService;
        _htmlHelpers = htmlHelpers;
        _currentDateTime = currentDateTime;
        _configuration = configuration;
    }

    public async Task<AccountHomeViewModel> GetAccountHomeViewModel(int providerId)
    {
        var result = false;
        try
        {
            result = await _authorizationService.IsAuthorizedAsync(ProviderFeature.FlexiblePaymentsPilot);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e,"Unable to get authorization for feature");
        }
        
        try
        {

            DateTime.TryParse(_configuration.TraineeshipCutOffDate, out var traineeshipCutOffDate);
            var showTraineeshipLink =
                traineeshipCutOffDate != DateTime.MinValue && traineeshipCutOffDate > _currentDateTime.Now;
            
            
            _logger.LogInformation("Getting provider {ProviderId}", providerId);

            var providerResponse = await _mediator.Send(new GetProviderQueryRequest { UKPRN = providerId });
            
            return new AccountHomeViewModel
            {
                AccountStatus = AccountStatus.Active,
                ProviderName = providerResponse.ProvidersView.Provider.ProviderName,
                ProviderId = providerId,
                ShowAcademicYearBanner = false,
                ShowTraineeshipLink = showTraineeshipLink,
                ShowEarningsReport = result,
                BannerContent = _htmlHelpers.GetClientContentByType("banner", useLegacyStyles: true),
                CovidSectionContent = _htmlHelpers.GetClientContentByType("covid_section", useLegacyStyles: true)
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,"Provider {ProviderId} details not found in provider information service", providerId);

            return new AccountHomeViewModel { AccountStatus = AccountStatus.NoAgreement };
        }
    }

    public async Task<NotificationSettingsViewModel> GetNotificationSettings(string userRef, string email)
    {
        _logger.LogInformation("Getting setting for user {UserRef}", userRef);

        var response = await _mediator.Send(new GetUserNotificationSettingsQuery
        {
            UserRef = userRef,
            Email = email
        });

        var model = new NotificationSettingsViewModel
        {
            NotificationSettings = Map(response.NotificationSettings)
        };

        _logger.LogTrace("Found {NotificationSettingsCount} settings for user {UserRef}", response.NotificationSettings.Count, userRef);

        return model;
    }

    public async Task UpdateNotificationSettings(NotificationSettingsViewModel model)
    {
        var setting = model.NotificationSettings.First();
        _logger.LogInformation("Updating setting for user {UserRef}", setting.UserRef);

        await _mediator.Send(new UpdateUserNotificationSettingsCommand
        {
            UserRef = setting.UserRef,
            ReceiveNotifications = setting.ReceiveNotifications
        });

        _logger.LogTrace("Updated receive notification to {ReceiveNotifications} for user {UserRef}", setting.ReceiveNotifications, setting.UserRef);
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

    private static IList<UserNotificationSetting> Map(IEnumerable<Domain.Models.Settings.UserNotificationSetting> notificationSettings)
    {
        if (notificationSettings == null) return new List<UserNotificationSetting>(0);

        return
            notificationSettings.Select(m =>
                    new UserNotificationSetting { UserRef = m.UserRef, ReceiveNotifications = m.ReceiveNotifications })
                .ToList();
    }

    private static SendNotificationCommand BuildNotificationCommand(string emailAddress, string userDisplayName, string urlToSettingsPage)
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
}