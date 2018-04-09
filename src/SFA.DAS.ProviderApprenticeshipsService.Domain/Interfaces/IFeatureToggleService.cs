using FeatureToggle;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IFeatureToggleService
    {
        IFeatureToggle Get<T>() where T : SimpleFeatureToggle, new();
    }
}
