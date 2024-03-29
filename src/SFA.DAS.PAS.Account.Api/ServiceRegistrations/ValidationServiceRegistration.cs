﻿using FluentValidation;
using SFA.DAS.PAS.Account.Application.Commands.SendNotification;

namespace SFA.DAS.PAS.Account.Api.ServiceRegistrations;

public static class ValidationServiceRegistration
{
    public static IServiceCollection AddApiValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<SendNotificationCommand>, SendNotificationCommandValidator>();

        return services;
    }
}