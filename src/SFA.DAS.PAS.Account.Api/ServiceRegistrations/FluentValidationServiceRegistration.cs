using FluentValidation;
using SFA.DAS.PAS.Account.Application.Commands.SendNotification;

namespace SFA.DAS.PAS.Account.Api.ServiceRegistrations;

public static class FluentValidationServiceRegistration
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<SendNotificationCommand>, SendNotificationCommandValidator>();

        return services;
    }
}