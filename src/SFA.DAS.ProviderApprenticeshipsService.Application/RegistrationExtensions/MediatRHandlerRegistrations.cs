using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteRegisteredUser;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UnsubscribeNotification;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpsertRegisteredUser;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetClientContent;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUser;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUserNotificationSettings;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions
{
    public static class MediatRHandlerRegistrations
    {
        public static IServiceCollection AddMediatRHandlers(this IServiceCollection services)
        {
            services.AddMediatR(typeof(GetClientContentRequestHandler));
            services.AddMediatR(typeof(GetCommitmentAgreementsQueryHandler));
            services.AddMediatR(typeof(GetProviderQueryHandler));
            services.AddMediatR(typeof(GetUserHandler));
            services.AddMediatR(typeof(GetUserNotificationSettingsHandler));

            services.AddMediatR(typeof(DeleteRegisteredUserCommandHandler));
            services.AddMediatR(typeof(SendNotificationCommandHandler));
            services.AddMediatR(typeof(UnsubscribeNotificationHandler));
            services.AddMediatR(typeof(UpdateUserNotificationSettingsHandler));
            services.AddMediatR(typeof(UpsertRegisteredUserCommandHandler));

            return services;
        }
    }
}
