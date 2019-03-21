using System;
using System.Threading;
using System.Threading.Tasks;
using FeatureToggle;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderHasRelationshipWithPermission;
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
        private Mock<IFeatureToggle> _recruitFeatureToggle;

        [SetUp]
        public void Arrange()
        {
            _recruitFeatureToggle = new Mock<IFeatureToggle>();
            _recruitFeatureToggle.Setup(x => x.FeatureEnabled).Returns(true);
            _featureToggleService = new Mock<IFeatureToggleService>();
            _featureToggleService.Setup(x => x.Get<Recruit>()).Returns(() => _recruitFeatureToggle.Object);

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

            _mediator.Setup(x =>
                    x.Send(It.Is<GetProviderHasRelationshipWithPermissionQueryRequest>(r =>
                            r.Permission == Operation.CreateCohort),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetProviderHasRelationshipWithPermissionQueryResponse());

            _currentDateTime = new Mock<ICurrentDateTime>();

            _orchestrator = new AccountOrchestrator(
                _mediator.Object,
                Mock.Of<ILog>(),
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

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenDisplayOfRecruitmentLinkIsSubjectToFeatureToggle(bool toggled)
        {
            _recruitFeatureToggle.Setup(x => x.FeatureEnabled).Returns(toggled);
            var model = await _orchestrator.GetAccountHomeViewModel(1);
            Assert.AreEqual(toggled, model.ShowRecruitLink);
        }
    }
}