﻿#define NETFRAMEWORK

#if NETFRAMEWORK
namespace SFA.DAS.ProviderUrlHelper
{
    public interface ILinkGenerator
    {
        string ProviderCommitmentsLink(string path);
        string ProviderApprenticeshipServiceLink(string path);
        string ReservationsLink(string path);
        string RecruitLink(string path);
        string RegistrationLink(string path);
    }
}
#endif
