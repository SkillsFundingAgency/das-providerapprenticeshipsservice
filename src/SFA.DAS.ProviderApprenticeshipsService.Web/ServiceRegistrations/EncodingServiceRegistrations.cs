using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SFA.DAS.Encoding;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations;

public static class EncodingServiceRegistrations
{
    public static IServiceCollection AddEncodingServices(this IServiceCollection services, IConfiguration configuration) 
    {
        // To be confirmed if encodingServices are used at all or can be deleted?
        var encodingConfigJson = configuration.GetSection("SFA.DAS.Encoding").Value;
        var encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(encodingConfigJson);
        services.AddSingleton(encodingConfig);

        services.AddSingleton<IEncodingService, EncodingService>();

        return services;
    }
}