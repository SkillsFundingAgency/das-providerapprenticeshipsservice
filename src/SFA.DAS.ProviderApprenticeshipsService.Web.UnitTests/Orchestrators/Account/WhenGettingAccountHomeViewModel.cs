using System;
using System.Threading;
using System.Threading.Tasks;
using FeatureToggle;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Features;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.FeatureToggles;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Account
{
    [TestFixture]
    public class WhenGettingAccountHomeViewModel
    {
        private AccountOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ICurrentDateTime> _currentDateTime;
        private Mock<IFeatureToggleService> _featureToggleService;
        private Mock<IAuthorizationService> _authorizationService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator
                .Setup(x => x.Send(It.IsAny<GetProviderQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProviderQueryResponse
                {
                    ProvidersView = new ProvidersView
                    {
                        CreatedDate = new DateTime(2000, 1, 1),
                        Provider = new Provider()
                    }
                });

            _currentDateTime = new Mock<ICurrentDateTime>();

            _featureToggleService = new Mock<IFeatureToggleService>();
            var featureToggle = new Mock<IFeatureToggle>();
            _featureToggleService.Setup(x => x.Get<Traineeships>()).Returns(featureToggle.Object);
            _authorizationService = new Mock<IAuthorizationService>();

            _orchestrator = new AccountOrchestrator(
                _mediator.Object,
                Mock.Of<ILog>(),
                _featureToggleService.Object,
                _authorizationService.Object
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
        public async Task Then_Set_Traineeship_Enabled_Flag(bool featureToggleSetting, bool expectedResult)
        {
            var cloudConfigToggleProviderMock = new Mock<IBooleanToggleValueProvider>();
            cloudConfigToggleProviderMock.Setup(x => x.EvaluateBooleanToggleValue(It.IsAny<Traineeships>())).Returns(featureToggleSetting);
            _featureToggleService.Setup(x => x.Get<Traineeships>()).Returns(new Traineeships { ToggleValueProvider = cloudConfigToggleProviderMock.Object });
            
            var model = await _orchestrator.GetAccountHomeViewModel(1);

            model.ShowTraineeshipLink.Should().Be(expectedResult);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Then_ShowEarningsReport_Is_Set_From_Authorization_Service(bool expectedResult)
        {
            _authorizationService.Setup(x => x.IsAuthorized(ProviderFeature.FlexiblePaymentsPilot)).Returns(expectedResult);
            var model = await _orchestrator.GetAccountHomeViewModel(1);
            model.ShowEarningsReport.Should().Be(expectedResult);
        }
    }
}