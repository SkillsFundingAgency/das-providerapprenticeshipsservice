using System.Web.Mvc;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.FeatureToggles;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderUrlHelper;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Controllers
{
    [TestFixture]
    public class ManageApprenticesControllerTests
    {
        [Test, MoqAutoData]
        public void WhenManageApprenticesV2FeatureToggleIsTurnedOnThenRoutesToProviderCommitments(
            ManageApprenticesOrchestrator orchestrator,
            [Frozen]Mock<FeatureToggleService> featureToggleService,
            [Frozen]Mock<LinkGenerator> linkGenerator,
            [Frozen]Mock<CookieStorageService<FlashMessageViewModel>> flashMessage)
            
        {
            //Arrange
            var controller = new ManageApprenticesController(orchestrator, flashMessage.Object, featureToggleService.Object, linkGenerator.Object);
            
            var providerId = 1;

            featureToggleService.Setup(x => x.Get<ProviderManageApprenticesV2>().FeatureEnabled).Returns(true);
            
            //Act
            var actual = controller.Index(providerId, new ApprenticeshipFiltersViewModel());

            //Assert
            Assert.IsInstanceOf<RedirectResult>(actual.Result);
        }

        [Test, MoqAutoData]
        public void WhenApprenticeDetailsV2FeatureToggleIsTurnedOnThenRoutesToProviderCommitments(
            ManageApprenticesOrchestrator orchestrator,
            [Frozen]Mock<FeatureToggleService> featureToggleService,
            [Frozen]Mock<LinkGenerator> linkGenerator,
            [Frozen]Mock<CookieStorageService<FlashMessageViewModel>> flashMessage)

        {
            //Arrange
            var controller = new ManageApprenticesController(orchestrator, flashMessage.Object, featureToggleService.Object, linkGenerator.Object);

            var providerId = 1;

            featureToggleService.Setup(x => x.Get<ApprenticeDetailsV2>().FeatureEnabled).Returns(true);

            //Act
            var actual = controller.Details(providerId, "XXXX");

            //Assert
            Assert.IsInstanceOf<RedirectResult>(actual.Result);
        }
    }
}