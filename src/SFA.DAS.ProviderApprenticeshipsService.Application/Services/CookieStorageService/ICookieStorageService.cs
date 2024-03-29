﻿namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services.CookieStorageService;

public interface ICookieStorageService<T>
{
    void Create(T item, string cookieName, int expiryDays = 1);
    void Delete(string cookieName);
    T Get(string cookieName);
    void Update(string cookieName, T item);
}