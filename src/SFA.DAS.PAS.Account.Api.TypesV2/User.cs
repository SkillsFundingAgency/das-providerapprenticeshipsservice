﻿namespace SFA.DAS.PAS.Account.Api.Types
{
    public class User
    {
        public string UserRef { get; set; }

        public string EmailAddress { get; set; }

        public bool ReceiveNotifications { get; set; }
    }
}
