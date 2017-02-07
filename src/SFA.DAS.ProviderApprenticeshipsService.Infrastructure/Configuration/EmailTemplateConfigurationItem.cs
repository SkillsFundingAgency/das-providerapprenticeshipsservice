using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration
{
    public class EmailTemplateConfigurationItem
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public EmailTemplateType TemplateType { get; set; }

        public string Key { get; set; }
    }
}
