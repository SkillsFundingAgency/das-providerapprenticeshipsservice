﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.IdamsUser;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserProfile;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;

public interface IUserRepository
{
    Task Upsert(User user);
    Task<User> GetUser(string userRef);
    Task<IEnumerable<User>> GetUsers(long ukprn);
    Task DeleteUser(string userRef);
    Task SyncIdamsUsers(long ukprn, List<IdamsUser> idamsUsers);
    Task<User> GetUserByEmail(string email);
}