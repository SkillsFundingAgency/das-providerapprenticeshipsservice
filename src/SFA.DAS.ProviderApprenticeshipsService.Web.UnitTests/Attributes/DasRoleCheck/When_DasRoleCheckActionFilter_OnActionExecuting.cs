using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Attributes.DasRoleCheck
{
    [TestFixture]
    public class When_DasRoleCheckActionFilter_OnActionExecuting
    {
        private readonly DasRoleCheckActionFilter Sut = new DasRoleCheckActionFilter();
        private ActionExecutingContext ActionExecutingContext;

        private void Arrange(string[] serviceClaims, bool isAuthenticated, string controllerAttribute, string methodAttribute)
        {
            var claimsIdentity = new Mock<ClaimsIdentity>();
            claimsIdentity.Setup(r => r.Claims)
                .Returns(serviceClaims
                    .Select(serviceClaim => new Claim("http://schemas.portal.com/service", serviceClaim)));

            claimsIdentity.Setup(r => r.IsAuthenticated)
                .Returns(isAuthenticated);

            var principle = new Mock<IPrincipal>();
            principle
                .Setup(r => r.Identity)
                .Returns(claimsIdentity.Object);

            var httpContextBase = new Mock<HttpContextBase>();
            httpContextBase
                .Setup(r => r.User)
                .Returns(principle.Object);

            var contollerContext = new ControllerContext(
                httpContextBase.Object,
                new Mock<RouteData>().Object,
                new Mock<ControllerBase>().Object
            );

            var controllerDescriptor = new Mock<ControllerDescriptor>();

            controllerDescriptor
                .Setup(r => r.IsDefined(It.Is<Type>(t => t.Equals(typeof(DasRoleCheckAttribute))), It.IsAny<bool>()))
                .Returns(controllerAttribute == nameof(DasRoleCheckAttribute));

            controllerDescriptor
                .Setup(r => r.IsDefined(It.Is<Type>(t => t.Equals(typeof(DasRoleCheckExemptAttribute))), It.IsAny<bool>()))
                .Returns(controllerAttribute == nameof(DasRoleCheckExemptAttribute));

            var actionDescriptor = new Mock<ActionDescriptor>();

            actionDescriptor
                .Setup(r => r.IsDefined(It.Is<Type>(t => t.Equals(typeof(DasRoleCheckAttribute))), It.IsAny<bool>()))
                .Returns(methodAttribute == nameof(DasRoleCheckAttribute));

            actionDescriptor
                .Setup(r => r.IsDefined(It.Is<Type>(t => t.Equals(typeof(DasRoleCheckExemptAttribute))), It.IsAny<bool>()))
                .Returns(methodAttribute == nameof(DasRoleCheckExemptAttribute));

            actionDescriptor
                .Setup(r => r.ControllerDescriptor)
                .Returns(controllerDescriptor.Object);

            ActionExecutingContext = new ActionExecutingContext(
                contollerContext,
                actionDescriptor.Object,
                new Dictionary<string, object>());
        }
        
        
        [TestCaseSource(nameof(ValidatesServiceClaimCases))]
        public void ValidateDasRole_ValidatesServiceClaim(string[] serviceClaims, bool isAuthenticated, string controllerAttribute, string methodAttribute, HttpStatusCode httpStatusCode)
        {
            // Arrange
            Arrange(serviceClaims, isAuthenticated, controllerAttribute, methodAttribute);

            // Act
            Action act = () => Sut.OnActionExecuting(ActionExecutingContext);
            
            // Assert
            if (httpStatusCode == HttpStatusCode.OK)
            {
                act.Should().NotThrow<Exception>();
            }
            else
            {
                act.Should().Throw<HttpException>()
                    .And.GetHttpCode().Should().Be((int)httpStatusCode);
            }
        }

        static object[] ValidatesServiceClaimCases =
        {
            new object[] { new string[] { "DAA", "SRV" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAA", "SRV" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "DAA", "SRV" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAA", "SRV" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAA", "SRV" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAA", "SRV" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "DAA", "SRV" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAA", "SRV" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "DAA", "SRV" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "DAA", "SRV" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},

            new object[] { new string[] { "SRV", "DAA" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "SRV", "DAA" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},

            new object[] { new string[] { "SRV", "DAA", "VRS" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA", "VRS" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA", "VRS" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA", "VRS" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA", "VRS" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA", "VRS" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA", "VRS" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA", "VRS" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAA", "VRS" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "SRV", "DAA", "VRS" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},

            new object[] { new string[] { "DAB", "SRV" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAB", "SRV" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "DAB", "SRV" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAB", "SRV" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAB", "SRV" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAB", "SRV" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "DAB", "SRV" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAB", "SRV" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "DAB", "SRV" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "DAB", "SRV" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},

            new object[] { new string[] { "SRV", "DAB" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "SRV", "DAB" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},

            new object[] { new string[] { "SRV", "DAB", "VRS" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB", "VRS" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB", "VRS" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB", "VRS" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB", "VRS" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB", "VRS" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB", "VRS" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB", "VRS" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAB", "VRS" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "SRV", "DAB", "VRS" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},

            new object[] { new string[] { "DAC", "SRV" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAC", "SRV" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "DAC", "SRV" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAC", "SRV" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAC", "SRV" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAC", "SRV" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "DAC", "SRV" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAC", "SRV" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "DAC", "SRV" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "DAC", "SRV" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},

            new object[] { new string[] { "SRV", "DAC" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAC" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAC" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAC" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAC" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAC" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAC" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAC" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAC" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "SRV", "DAC" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},

            new object[] { new string[] { "VRS", "DAC", "SRV" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAC", "SRV" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAC", "SRV" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAC", "SRV" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAC", "SRV" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAC", "SRV" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAC", "SRV" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAC", "SRV" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAC", "SRV" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "VRS", "DAC", "SRV" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},

            new object[] { new string[] { "DAV", "SRV" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAV", "SRV" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "DAV", "SRV" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAV", "SRV" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAV", "SRV" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAV", "SRV" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "DAV", "SRV" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "DAV", "SRV" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "DAV", "SRV" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "DAV", "SRV" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},

            new object[] { new string[] { "SRV", "DAV" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAV" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAV" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAV" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAV" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAV" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAV" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAV" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "DAV" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "SRV", "DAV" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},

            new object[] { new string[] { "VRS", "DAV", "SRV" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAV", "SRV" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAV", "SRV" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAV", "SRV" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAV", "SRV" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAV", "SRV" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAV", "SRV" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAV", "SRV" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "DAV", "SRV" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "VRS", "DAV", "SRV" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},

            new object[] { new string[] { "" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},
            new object[] { new string[] { "" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},

            new object[] { new string[] { "", "SRV" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "", "SRV" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},
            new object[] { new string[] { "", "SRV" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "", "SRV" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "", "SRV" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "", "SRV" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "", "SRV" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "", "SRV" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "", "SRV" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "", "SRV" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},

            new object[] { new string[] { "SRV", "" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "SRV", "" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},
            new object[] { new string[] { "SRV", "" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "SRV", "" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "SRV", "" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},

            new object[] { new string[] { "VRS", "SRV", "" }, true, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "VRS", "SRV", "" }, true, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden},
            new object[] { new string[] { "VRS", "SRV", "" }, true, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "SRV", "" }, false, "", "", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "SRV", "" }, true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "SRV", "" }, true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "SRV", "" }, false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "SRV", "" }, false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK},
            new object[] { new string[] { "VRS", "SRV", "" }, false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden},
            new object[] { new string[] { "VRS", "SRV", "" }, false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden}
        };
    }
}
