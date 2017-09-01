using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Azure;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration
{
    public class CurrentDatePolicy : ConfiguredInstancePolicy
    {
        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var constructorParameter =
                instance?.Constructor?.GetParameters().FirstOrDefault(x => x.ParameterType == typeof(ICurrentDateTime));
            
            if (constructorParameter != null)
            {
                var param = new CurrentDateTime(GetCurrentTimeFromConfiguration());
                instance.Dependencies.AddForConstructorParameter(constructorParameter, param);
            }
        }

        private DateTime? GetCurrentTimeFromConfiguration()
        {
            var cloudCurrentTime = CloudConfigurationManager.GetSetting("CurrentTime");

            var result = default(DateTime?);
            DateTime parsedTime;

            if (DateTime.TryParse(cloudCurrentTime, out parsedTime))
            {
                result = parsedTime;
            }

            return result;
        }
    }
}