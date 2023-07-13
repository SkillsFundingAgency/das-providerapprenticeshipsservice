using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations;

public static class EntityFrameworkStartup
{
    public static IServiceCollection AddEntityFramework(this IServiceCollection services, ProviderApprenticeshipsServiceConfiguration config)
    {
        var connection =  SqlConnectionFactory.GetConnectionAsync(config.DatabaseConnectionString).Result;

        services.AddScoped(p =>
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            ProviderApprenticeshipsDbContext dbContext;

            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<ProviderApprenticeshipsDbContext>().UseSqlServer(connection);
                dbContext = new ProviderApprenticeshipsDbContext(connection, optionsBuilder.Options, config, azureServiceTokenProvider);
            }
            catch (KeyNotFoundException)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ProviderApprenticeshipsDbContext>().UseSqlServer(connection);
                dbContext = new ProviderApprenticeshipsDbContext(optionsBuilder.Options);
            }

            return dbContext;
        });

        return services;
    }
}