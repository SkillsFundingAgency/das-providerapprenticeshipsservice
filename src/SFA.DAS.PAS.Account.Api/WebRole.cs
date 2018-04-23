using Microsoft.WindowsAzure.ServiceRuntime;
using NLog;
using System;

namespace SFA.DAS.PAS.Account.Api
{
    public class WebRole : RoleEntryPoint
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public override bool OnStart()
        {
            try
            {
                return base.OnStart();
            }
            catch (Exception e)
            {
                Logger.Fatal(e);
                throw e;
            }
        }
}