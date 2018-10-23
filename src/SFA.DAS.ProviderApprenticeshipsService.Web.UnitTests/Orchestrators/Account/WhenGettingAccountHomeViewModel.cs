using System;
using System.Threading.Tasks;
using FeatureToggle;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.FeatureToggles;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Account
{
    [TestFixture]
    public class WhenGettingAccountHomeViewModel
    {
        private AccountOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ICurrentDateTime> _currentDateTime;
        private Mock<IFeatureToggleService> _featureToggleService;
        private Mock<IFeatureToggle> _featureToggle;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator
                .Setup(x => x.SendAsync(It.IsAny<GetProviderQueryRequest>()))
                .ReturnsAsync(new GetProviderQueryResponse
                {
                    ProvidersView = new ProvidersView
                    {
                        CreatedDate = new DateTime(2000, 1, 1),
                        Provider = new Provider()
                    }
                });

            _featureToggle = new Mock<IFeatureToggle>();
            _featureToggleService = new Mock<IFeatureToggleService>();
            _featureToggleService.Setup(x => x.Get<ProviderRelationships>()).Returns(_featureToggle.Object);
            
            _currentDateTime = new Mock<ICurrentDateTime>();

            _orchestrator = new AccountOrchestrator(
                _mediator.Object,
                Mock.Of<ILog>(),
                _currentDateTime.Object,
                _featureToggleService.Object
            );
        }

        [TestCase("2018-10-18", false, TestName = "Banner permanently hidden")]
        public async Task ThenDisplayOfAcademicYearBannerIsDetermined(DateTime now, bool expectShowBanner)
        {
            _currentDateTime.Setup(x => x.Now).Returns(now);

            var model = await _orchestrator.GetAccountHomeViewModel(1);

            model.ShowAcademicYearBanner.Should().Be(expectShowBanner);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task ThenDisplayOfCreateCohortLinkIsDetermined(bool featureEnabled, bool expectShowLink)
        {
            //Arrange
            _featureToggle.Setup(x => x.FeatureEnabled).Returns(featureEnabled);

            //Act
            var model = await _orchestrator.GetAccountHomeViewModel(1);

            //Assert
            model.ShowCreateCohortLink.Should().Be(expectShowLink);
        }
    }
}