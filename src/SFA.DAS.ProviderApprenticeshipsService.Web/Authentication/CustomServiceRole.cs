using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authentication
{
    public class CustomServiceRole : ICustomServiceRole
    {
        public string RoleClaimType => DasClaimTypes.Service;

        // <inherit-doc/>
        public CustomServiceRoleValueType RoleValueType => CustomServiceRoleValueType.Code;
    }
}
