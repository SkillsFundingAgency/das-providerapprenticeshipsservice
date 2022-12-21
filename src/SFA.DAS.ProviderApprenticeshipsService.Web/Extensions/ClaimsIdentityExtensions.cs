using System;
using System.Linq;
using System.Security.Claims;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Web.Constants;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public static class ClaimsIdentityExtensions
    {
        public static void MapClaimToRoles(this ClaimsIdentity identity)
        {
            foreach (var serviceClaim in identity.Claims.Where(c => c.Type == DasClaimTypes.Service))
            {
                if(Enum.TryParse(serviceClaim.Value, true, out ServiceClaim claim))
                {
                    identity.AddRolesFor(claim);
                }
            }
        }

        private static void AddRolesFor(this ClaimsIdentity identity, ServiceClaim claim)
        {
            switch (claim)
            {
                case ServiceClaim.DAA: 
                    identity.AddRole(RoleNames.HasAccountOwnerPermission);
                    identity.AddRole(RoleNames.HasContributorWithApprovalOrAbovePermission);
                    identity.AddRole(RoleNames.HasContributorOrAbovePermission);
                    identity.AddRole(RoleNames.HasViewerOrAbovePermission);
                    identity.AddRole(RoleNames.DasPermission);
                    break;
                case ServiceClaim.DAB:
                    identity.AddRole(RoleNames.HasContributorWithApprovalOrAbovePermission);
                    identity.AddRole(RoleNames.HasContributorOrAbovePermission);
                    identity.AddRole(RoleNames.HasViewerOrAbovePermission);
                    identity.AddRole(RoleNames.DasPermission);
                    break;
                case ServiceClaim.DAC:
                    identity.AddRole(RoleNames.HasContributorOrAbovePermission);
                    identity.AddRole(RoleNames.HasViewerOrAbovePermission);
                    identity.AddRole(RoleNames.DasPermission);
                    break;
                case ServiceClaim.DAV:
                    identity.AddRole(RoleNames.HasViewerOrAbovePermission);
                    identity.AddRole(RoleNames.DasPermission);
                    break;
            }
        }

        private static void AddRole(this ClaimsIdentity identity, string role)
        {
            if (!identity.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == role))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
        }

        public static string GetClaimValue(this ClaimsIdentity claimsIdentity, string claimName)
        {
            var claimValue = claimsIdentity.Claims.Where(c => c.Type.Contains(claimName)).Select(c => c.Value).SingleOrDefault();
            return claimValue ?? string.Empty;
        }
    }
}
