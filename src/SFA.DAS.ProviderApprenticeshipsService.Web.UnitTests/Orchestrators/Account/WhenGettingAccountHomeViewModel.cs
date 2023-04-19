using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Services;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Features;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Account;

[TestFixture]
public class WhenGettingAccountHomeViewModel
{
    private AccountOrchestrator _orchestrator;
    private Mock<IMediator> _mediator;
    private Mock<ICurrentDateTime> _currentDateTime;
    private Mock<IAuthorizationService> _authorizationService;
    private Mock<IHtmlHelpers> _htmlHelpers;

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
                    Provider = new Domain.Models.ApprenticeshipProvider.Provider()
                }
            });

        _currentDateTime = new Mock<ICurrentDateTime>();
        _authorizationService = new Mock<IAuthorizationService>();
        _htmlHelpers = new Mock<IHtmlHelpers>();

        _orchestrator = new AccountOrchestrator(
            _mediator.Object,
            Mock.Of<ILogger<AccountOrchestrator>>(),
            _authorizationService.Object,
            _htmlHelpers.Object
        );
    }

    [TestCase("2018-10-18", false, TestName = "Banner permanently hidden")]
    public async Task ThenDisplayOfAcademicYearBannerIsDetermined(DateTime now, bool expectShowBanner)
    {
        _currentDateTime.Setup(x => x.Now).Returns(now);

        var model = await _orchestrator.GetAccountHomeViewModel(1);

        model.ShowAcademicYearBanner.Should().Be(expectShowBanner);
    }

    [Test]
    public async Task Then_Set_Traineeship_Enabled_Flag()
    {
        var model = await _orchestrator.GetAccountHomeViewModel(1);

        model.ShowTraineeshipLink.Should().Be(true);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Then_ShowEarningsReport_Is_Set_From_Authorization_Service(bool expectedResult)
    {
        // Arrange
        _authorizationService.Setup(x => x.IsAuthorized(ProviderFeature.FlexiblePaymentsPilot)).Returns(expectedResult);
        // Act
        var model = await _orchestrator.GetAccountHomeViewModel(1);

        // Assert
        model.ShowEarningsReport.Should().Be(expectedResult);
    }

    [TestCase("bannercontent")]
    public async Task Then_BannerContent_Is_Set_From_HtmlHelpers(string expectedBannerContentString)
    {
        // Arrange
        var expectedBannerContent = new HtmlString(expectedBannerContentString);

        _htmlHelpers.Setup(x => x.GetClientContentByType(It.IsAny<string>(), It.IsAny<bool>())).Returns(expectedBannerContent);

        // Act
        var model = await _orchestrator.GetAccountHomeViewModel(1);

        // Assert
        model.BannerContent.Should().Be(expectedBannerContent);
    }

    [TestCase("covidsectioncontent")]
    public async Task Then_CovidSectionContent_Is_Set_From_HtmlHelpers(string expectedCovidSectionContentString)
    {
        // Arrange
        var expectedCovidSectionContent = new HtmlString(expectedCovidSectionContentString);
        _htmlHelpers.Setup(x => x.GetClientContentByType(It.IsAny<string>(), It.IsAny<bool>())).Returns(expectedCovidSectionContent);

        // Act
        var model = await _orchestrator.GetAccountHomeViewModel(1);

        // Assert
        model.CovidSectionContent.Should().Be(expectedCovidSectionContent);
    }
}