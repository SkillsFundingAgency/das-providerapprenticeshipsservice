﻿using Microsoft.AspNetCore.Http;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

public interface ICookieService<T>
{
    void Create(HttpContextAccessor contextAccessor, string name, T content, int expireDays);

    void Update(HttpContextAccessor contextAccessor, string name, T content);

    void Delete(HttpContextAccessor contextAccessor, string name);

    T Get(HttpContextAccessor contextAccessor, string name);
}