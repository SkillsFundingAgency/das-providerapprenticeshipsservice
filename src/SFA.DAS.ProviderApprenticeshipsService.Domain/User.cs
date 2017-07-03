namespace SFA.DAS.ProviderApprenticeshipsService.Domain
{
    public class User
    {
        public long Id { get; set; }
        public string UserRef { get; set; }
        public string DisplayName { get; set; }
        public long Ukprn { get; set; }
        public string Email { get; set; }
    }
}
