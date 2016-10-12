namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface ICache
    {
        bool Exists(string key);
        T GetCustomValue<T>(string key);
        void SetCustomValue<T>(string key, T customType, int secondsInCache = 300);
    }
}