using SFA.DAS.PAS.Account.Application.Queries.GetAccountUsers;

namespace SFA.DAS.PAS.Account.Api.ServiceRegistrations;

public static class MediatRHandlerServiceRegistrations
{
    public static IServiceCollection AddMediatRHandlers(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetAccountUsersHandler>());

        return services;
    }
}