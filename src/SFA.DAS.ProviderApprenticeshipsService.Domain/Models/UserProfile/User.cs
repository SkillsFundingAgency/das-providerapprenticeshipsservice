namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserProfile
{
    public class User
    {
        public long Id { get; set; }
        public string UserRef { get; set; }
        public string DisplayName { get; set; }
        public long Ukprn { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        public UserType UserType { get; set; }
        public string ServiceClaim { get; set; }
    }
}
