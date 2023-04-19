using SFA.DAS.PAS.Account.Application.Commands.SendNotification;
using SFA.DAS.PAS.Account.Application.Queries.GetAccountUsers;
using SFA.DAS.PAS.Account.Application.Queries.GetProviderAgreement;
using SFA.DAS.PAS.Account.Application.Queries.GetUserNotificationSettings;

namespace SFA.DAS.PAS.Account.Api.ServiceRegistrations;

public static class MediatRHandlerServiceRegistrations
{
    public static IServiceCollection AddMediatRHandlers(this IServiceCollection services)
    {
        services.AddMediatR(typeof(GetAccountUsersHandler));
        services.AddMediatR(typeof(GetProviderAgreementQueryHandler));
        services.AddMediatR(typeof(GetUserNotificationSettingsHandler));
        services.AddMediatR(typeof(SendNotificationCommandHandler));

        return services;
    }
}