﻿using System.Threading.Tasks;

using SFA.DAS.PAS.Account.Api.Types;

namespace SFA.DAS.PAS.Account.Api.Client
{
    public interface IAccountApiClient
    {
        Task<User> GetUser(string userRef);
    }
}