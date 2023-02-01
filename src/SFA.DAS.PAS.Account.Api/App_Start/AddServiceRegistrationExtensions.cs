using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Application.Commands.SendNotification;
using SFA.DAS.PAS.Account.Application.Queries.GetAccountUsers;
using SFA.DAS.PAS.Account.Application.Queries.GetProviderAgreement;
using SFA.DAS.PAS.Account.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.PAS.Account.Api;

public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services)
    {
        services.AddMediatR(typeof(GetAccountUsersHandler));
        services.AddMediatR(typeof(GetProviderAgreementQueryHandler));
        services.AddMediatR(typeof(GetUserNotificationSettingsHandler));
        services.AddMediatR(typeof(SendNotificationCommandHandler));

        services.AddTransient<IAccountOrchestrator, AccountOrchestrator>();
        services.AddTransient<IEmailOrchestrator, EmailOrchestrator>();
        services.AddTransient<IUserOrchestrator, UserOrchestrator>();

        services.AddTransient<IBackgroundNotificationService, BackgroundNotificationService>();
        services.AddTransient<IProviderCommitmentsLogger, ProviderCommitmentsLogger>();

        services.AddTransient<IUserSettingsRepository, UserSettingsRepository>();
        services.AddTransient<IAgreementStatusQueryRepository, ProviderAgreementStatusRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        
        services.AddTransient<ICurrentDateTime, CurrentDateTime>();
    }
}