using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Attributes.DasRoleCheck
{
    public class When_DasRoleCheckActionFilter_OnActionExecuting
    {
        private readonly DasRoleCheckActionFilter Sut = new DasRoleCheckActionFilter();
        private ActionExecutingContext ActionExecutingContext;

        private void Arrange(string serviceClaim, bool isAuthenticated, string controllerAttribute, string methodAttribute)
        {
            var claimsIdentity = new Mock<ClaimsIdentity>();
            claimsIdentity.Setup(r => r.Claims)
                .Returns(new List<Claim> { new Claim("http://schemas.portal.com/service", serviceClaim) });

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
        
        
        [TestCase("DAA", true, "DasRoleCheckAttribute", "", HttpStatusCode.OK)]
        [TestCase("DAA", true, "", "DasRoleCheckAttribute", HttpStatusCode.OK)]
        [TestCase("DAA", true, "", "", HttpStatusCode.OK)]
        [TestCase("DAA", false, "", "", HttpStatusCode.OK)]
        [TestCase("DAA", true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK)]
        [TestCase("DAA", true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK)]
        [TestCase("DAA", false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK)]
        [TestCase("DAA", false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK)]
        [TestCase("DAA", false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden)]
        [TestCase("DAA", false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden)]
        [TestCase("DAB", true, "DasRoleCheckAttribute", "", HttpStatusCode.OK)]
        [TestCase("DAB", true, "", "DasRoleCheckAttribute", HttpStatusCode.OK)]
        [TestCase("DAB", true, "", "", HttpStatusCode.OK)]
        [TestCase("DAB", false, "", "", HttpStatusCode.OK)]
        [TestCase("DAB", true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK)]
        [TestCase("DAB", true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK)]
        [TestCase("DAB", false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK)]
        [TestCase("DAB", false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK)]
        [TestCase("DAB", false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden)]
        [TestCase("DAB", false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden)]
        [TestCase("DAC", true, "DasRoleCheckAttribute", "", HttpStatusCode.OK)]
        [TestCase("DAC", true, "", "DasRoleCheckAttribute", HttpStatusCode.OK)]
        [TestCase("DAC", true, "", "", HttpStatusCode.OK)]
        [TestCase("DAC", false, "", "", HttpStatusCode.OK)]
        [TestCase("DAC", true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK)]
        [TestCase("DAC", true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK)]
        [TestCase("DAC", false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK)]
        [TestCase("DAC", false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK)]
        [TestCase("DAC", false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden)]
        [TestCase("DAC", false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden)]
        [TestCase("DAV", true, "DasRoleCheckAttribute", "", HttpStatusCode.OK)]
        [TestCase("DAV", true, "", "DasRoleCheckAttribute", HttpStatusCode.OK)]
        [TestCase("DAV", true, "", "", HttpStatusCode.OK)]
        [TestCase("DAV", false, "", "", HttpStatusCode.OK)]
        [TestCase("DAV", true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK)]
        [TestCase("DAV", true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK)]
        [TestCase("DAV", false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK)]
        [TestCase("DAV", false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK)]
        [TestCase("DAV", false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden)]
        [TestCase("DAV", false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden)]
        [TestCase("", true, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden)]
        [TestCase("", true, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden)]
        [TestCase("", true, "", "", HttpStatusCode.OK)]
        [TestCase("", false, "", "", HttpStatusCode.OK)]
        [TestCase("", true, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK)]
        [TestCase("", true, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK)]
        [TestCase("", false, "DasRoleCheckExemptAttribute", "", HttpStatusCode.OK)]
        [TestCase("", false, "", "DasRoleCheckExemptAttribute", HttpStatusCode.OK)]
        [TestCase("", false, "DasRoleCheckAttribute", "", HttpStatusCode.Forbidden)]
        [TestCase("", false, "", "DasRoleCheckAttribute", HttpStatusCode.Forbidden)]
        public void ValidateDasRole_ValidatesServiceClaim(string serviceClaim, bool isAuthenticated, string controllerAttribute, string methodAttribute, HttpStatusCode httpStatusCode)
        {
            // Arrange
            Arrange(serviceClaim, isAuthenticated, controllerAttribute, methodAttribute);

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
    }
}
