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
                            r.Permission == Operation.CreateCohort),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetProviderHasRelationshipWithPermissionQueryResponse());

            _currentDateTime = new Mock<ICurrentDateTime>();

            _featureToggleService = new Mock<IFeatureToggleService>();
            
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

        [TestCase(true, true, TestName = "Bulk upload feature allowed.")]
        [TestCase(false, false, TestName = "Bulk upload  NOT feature allowed.")]
        public async Task ThenSetBulkUploadEnabledFeatureToggle(bool featureToggleSetting, bool expectedResult)
        {
            var cloudConfigToggleProviderMock = new Mock<IBooleanToggleValueProvider>();
            cloudConfigToggleProviderMock.Setup(x => x.EvaluateBooleanToggleValue(It.IsAny<BulkUploadV2>())).Returns(featureToggleSetting);
            _featureToggleService.Setup(x => x.Get<BulkUploadV2>()).Returns(new BulkUploadV2 { ToggleValueProvider = cloudConfigToggleProviderMock.Object });

            var model = await _orchestrator.GetAccountHomeViewModel(1);

            model.IsBulkUploadV2Enabled.Should().Be(expectedResult);
        }
    }
}