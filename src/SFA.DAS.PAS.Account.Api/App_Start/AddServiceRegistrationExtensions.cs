using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Application.Queries.GetAccountUsers;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.PAS.Account.Api;

public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services)
    {
        services.AddMediatR(typeof(GetAccountUsersQuery));
        
        services.AddTransient<IAccountOrchestrator, AccountOrchestrator>();
        services.AddTransient<IEmailOrchestrator, EmailOrchestrator>();
        services.AddTransient<IUserOrchestrator, UserOrchestrator>();

        services.AddTransient<IBackgroundNotificationService, BackgroundNotificationService>();

        services.AddTransient<IUserSettingsRepository, UserSettingsRepository>();
        services.AddTransient<IAgreementStatusQueryRepository, ProviderAgreementStatusRepository>();
        
        services.AddTransient<IUserRepository, UserRepository>();
        
        services.AddTransient<ICurrentDateTime, CurrentDateTime>();
    }
}