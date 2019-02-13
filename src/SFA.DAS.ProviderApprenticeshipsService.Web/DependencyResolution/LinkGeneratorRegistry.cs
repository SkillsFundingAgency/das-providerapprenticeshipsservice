using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderUrlHelper;
using StructureMap;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.DependencyResolution
{
    public class LinkGeneratorRegistry : Registry
    {
        public LinkGeneratorRegistry()
        {
            For<ILinkGenerator>().Use<LinkGenerator>().Singleton();
        }
    }
}