using Microsoft.OpenApi.Models;
using SFA.DAS.Api.Common.Infrastructure;

namespace SFA.DAS.PAS.Account.Api.ServiceRegistrations;

public static class SwaggerServiceRegistrations
{
    public static IServiceCollection AddDasSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            var securityScheme = new OpenApiSecurityScheme()
            {
                Description = "Access Token. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
            };

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "bearerAuth"
                        }
                    },
                    Array.Empty<string>()
                }
            };

            options.AddSecurityDefinition("bearerAuth", securityScheme);
            options.AddSecurityRequirement(securityRequirement);
            options.OperationFilter<SwaggerVersionHeaderFilter>();
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "PasAccountApi", Version = "v1" });
        });

        return services;
    }
}