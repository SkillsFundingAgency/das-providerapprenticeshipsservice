using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Web.Constants;
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

        [TestCase(ClaimName.Email, "someEmail@test.com")]
        [TestCase(ClaimName.GivenName, "some name")]
        [TestCase(ClaimName.Organisation, "DOE")]
        [TestCase(ClaimName.NameIdentifier, "Full Name")]
        [TestCase(ClaimName.RoleCode, "")]
        public void GetClaimValue_When_GivenClaimName_Return_Expected(string claimName, string expectedValue)
        {
            // arrange
            var identity = GetMockClaimsIdentity();

            // sut
            var actualValue = identity.GetClaimValue(claimName);

            // assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        private static ClaimsIdentity GetMockClaimsIdentity()
        {
            return new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimName.Email, "someEmail@test.com"),
                new Claim(ClaimName.GivenName, "some name"),
                new Claim(ClaimName.Organisation, "DOE"),
                new Claim(ClaimName.NameIdentifier, "Full Name"),
            }, "TestAuthType");
        }
    }
}
