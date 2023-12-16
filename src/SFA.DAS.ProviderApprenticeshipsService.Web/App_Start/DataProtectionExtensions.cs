using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using StackExchange.Redis;

namespace SFA.DAS.ProviderApprenticeshipsService.Web;

public static class AddDataProtectionExtensions
{
    public static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration)
    {
            
        var config = configuration.Get<ProviderApprenticeshipsServiceConfiguration>();

        if (config == null
            || string.IsNullOrEmpty(config.DataProtectionKeysDatabase)
            || string.IsNullOrEmpty(config.RedisConnectionString))
        {
            return services;
        }


        var redisConnectionString = config.RedisConnectionString;
        var dataProtectionKeysDatabase = config.DataProtectionKeysDatabase;

        var redis = ConnectionMultiplexer
            .Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

        services.AddDataProtection()
            .SetApplicationName("das-provider")
            .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

        return services;
    }
}