﻿using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.PAS.Account.Api
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
