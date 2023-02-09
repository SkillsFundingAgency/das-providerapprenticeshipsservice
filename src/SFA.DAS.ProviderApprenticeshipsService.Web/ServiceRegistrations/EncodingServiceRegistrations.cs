using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Encoding;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations
{
    public static class EncodingServiceRegistrations
    {
        public static IServiceCollection AddEncodingServices(this IServiceCollection services, IConfiguration configuration) 
        {
            // To be confirmed If SFA.DAS.Encoding can be referenced or need to create a Encoding Config class in this solution
            //
            // ALSO, to be confirmed if encodingServices are used at all or can be deleted?
            services.AddSingleton(configuration.Get<EncodingConfig>()); // for this to work, SFA.DAS.Encoding has been added to ConfigNames section in appsettings.json
            
            // uncomment the below if the above doesnt work
            // services.AddSingleton(JsonConvert.DeserializeObject<EncodingConfig>(configuration.GetSection(ConfigurationKeys.EncodingConfig).Value));

            services.AddSingleton<IEncodingService>(configuration.Get<EncodingService>());

            return services;
        }

        // replacing:
        // For<EncodingConfig>().Use(ctx => GetConfig(ctx)).Singleton();
        // For<IEncodingService>().Use<EncodingService>().Singleton();
    }
}
