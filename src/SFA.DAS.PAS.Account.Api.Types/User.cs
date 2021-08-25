namespace SFA.DAS.PAS.Account.Api.Types
{
    public class User
    {
        public string UserRef { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }
        public bool ReceiveNotifications { get; set; }
        public bool IsSuperUser { get; set; }
        public string ServiceClaim { get; set; }
    }
}
