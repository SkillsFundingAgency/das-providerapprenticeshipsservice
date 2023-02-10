using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Models.IdamsUser
{
    public class IdamsUser
    {
        public string Email { get; set; }
        public UserType UserType { get; set; }
    }
}
