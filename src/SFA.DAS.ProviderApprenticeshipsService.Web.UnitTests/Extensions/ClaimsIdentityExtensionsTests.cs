using System.Linq;
using System.Security.Claims;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Extensions
{
    [TestFixture]
    class ClaimsIdentityExtensionsTests
    {
        [TestCase(new string[] { "DAA" }, new string[] { nameof(RoleNames.DasPermission), nameof(RoleNames.HasViewerOrAbovePermission), nameof(RoleNames.HasContributorOrAbovePermission), nameof(RoleNames.HasContributorWithApprovalOrAbovePermission), nameof(RoleNames.HasAccountOwnerPermission) })]
        [TestCase(new string[] { "DAB" }, new string[] { nameof(RoleNames.DasPermission), nameof(RoleNames.HasViewerOrAbovePermission), nameof(RoleNames.HasContributorOrAbovePermission), nameof(RoleNames.HasContributorWithApprovalOrAbovePermission) })]
        [TestCase(new string[] { "DAC" }, new string[] { nameof(RoleNames.DasPermission), nameof(RoleNames.HasViewerOrAbovePermission), nameof(RoleNames.HasContributorOrAbovePermission) })]
        [TestCase(new string[] { "DAV" }, new string[] { nameof(RoleNames.DasPermission), nameof(RoleNames.HasViewerOrAbovePermission) })]
        [TestCase(new string[] { "DCS" }, new string[] { })]
        public void ValidateClaimsToRoleMapping(string[] serviceClaims, string[] expectedRoles)
        {
            var claims = serviceClaims.Select(a => new Claim(DasClaimTypes.Service, a));
            var identity = new ClaimsIdentity(claims, "TestAuthType");

            identity.MapClaimToRoles();

            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            CollectionAssert.AreEquivalent(expectedRoles, roles);
        }
    }
}
