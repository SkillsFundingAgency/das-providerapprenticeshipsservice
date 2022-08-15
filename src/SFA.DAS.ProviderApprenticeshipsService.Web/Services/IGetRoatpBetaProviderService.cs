using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Services
{
    public interface IGetRoatpBetaProviderService
    {
        //List<int> GetBetaProviderUkprns();
        bool IsUkprnEnabled(int ukprn);
    }
}