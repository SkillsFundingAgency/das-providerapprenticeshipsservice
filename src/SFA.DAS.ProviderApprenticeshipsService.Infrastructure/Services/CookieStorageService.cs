using Microsoft.AspNetCore.Http;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public class CookieStorageService<T> : ICookieStorageService<T>
    {
        private readonly ICookieService<T> _cookieService;
        private readonly HttpContextAccessor _httpContextAccessor;

        public CookieStorageService(ICookieService<T> cookieService, HttpContextAccessor httpContextBase)
        {
            _cookieService = cookieService;
            _httpContextAccessor = httpContextBase;
        }

        public void Create(T item, string cookieName, int expiryDays = 1)
        {
            _cookieService.Create(_httpContextAccessor, cookieName, item, expiryDays);
        }

        public T Get(string cookieName)
        {
            return _cookieService.Get(_httpContextAccessor, cookieName);
        }

        public void Delete(string cookieName)
        {
            _cookieService.Delete(_httpContextAccessor, cookieName);
        }

        public void Update(string cookieName, T item)
        {
            _cookieService.Update(_httpContextAccessor, cookieName, item);
        }
    }
}
