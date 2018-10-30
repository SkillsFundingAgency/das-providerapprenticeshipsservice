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
using SFA.DAS.ProviderRelationships.Types;

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
                            r.Permission == PermissionEnumDto.CreateCohort),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetProviderHasRelationshipWithPermissionQueryResponse());

            _featureToggle = new Mock<IFeatureToggle>();
            _featureToggleService = new Mock<IFeatureToggleService>();
            _featureToggleService.Setup(x => x.Get<Domain.Models.FeatureToggles.ProviderRelationships>()).Returns(_featureToggle.Object);
            
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

        [TestCase(true, true, true, Description = "Show link if feature is enabled and provider has relevant permission" )]
        [TestCase(true, false, false, Description = "Hide link if feature is enabled but provider does not have relevant permission")]
        [TestCase(false, true, false, Description = "Hide link if feature is disabled but provider has relevant permission")]
        [TestCase(false, false, false, Description = "Hide link if feature is disabled and provider does not has relevant permission")]
        public async Task ThenDisplayOfCreateCohortLinkIsDetermined(bool featureEnabled, bool hasPermission, bool expectShowLink)
        {
            //Arrange
            _featureToggle.Setup(x => x.FeatureEnabled).Returns(featureEnabled);

            _mediator.Setup(x =>
                    x.Send(It.Is<GetProviderHasRelationshipWithPermissionQueryRequest>(r =>
                            r.Permission == PermissionEnumDto.CreateCohort),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetProviderHasRelationshipWithPermissionQueryResponse{ HasPermission = hasPermission});

            //Act
            var model = await _orchestrator.GetAccountHomeViewModel(1);

            //Assert
            model.ShowCreateCohortLink.Should().Be(expectShowLink);
        }
    }
}