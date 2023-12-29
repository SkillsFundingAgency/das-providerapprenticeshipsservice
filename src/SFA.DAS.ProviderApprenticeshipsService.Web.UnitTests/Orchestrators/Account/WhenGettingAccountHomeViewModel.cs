using System.Globalization;
using Microsoft.Extensions.Logging;
using SFA.DAS.Authorization.Services;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Features;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
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
    private ProviderApprenticeshipsServiceConfiguration _configuration;

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
        _configuration = new ProviderApprenticeshipsServiceConfiguration();

        _orchestrator = new AccountOrchestrator(
            _mediator.Object,
            Mock.Of<ILogger<AccountOrchestrator>>(),
            _authorizationService.Object,
            _htmlHelpers.Object,
            _currentDateTime.Object,
            _configuration
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
    public async Task Then_Set_Traineeship_Enabled_Flag_Before_Configured_Date()
    {
        _configuration.TraineeshipCutOffDate = new DateTime(2023, 10, 30).ToString(CultureInfo.CurrentCulture);
        _currentDateTime.Setup(x => x.Now).Returns(new DateTime(2023, 10, 30).AddMilliseconds(-1));
        
        var model = await _orchestrator.GetAccountHomeViewModel(1);

        model.ShowTraineeshipLink.Should().Be(true);
    }
    
    [Test]
    public async Task Then_Traineeship_Link_Disabled_After_Configured_Date()
    {
        _configuration.TraineeshipCutOffDate = new DateTime(2023, 10, 30).ToString(CultureInfo.CurrentCulture);
        _currentDateTime.Setup(x => x.Now).Returns(new DateTime(2023, 10, 30).AddMilliseconds(1));
        
        var model = await _orchestrator.GetAccountHomeViewModel(1);

        model.ShowTraineeshipLink.Should().Be(false);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Then_ShowEarningsReport_Is_Set_From_Authorization_Service(bool expectedResult)
    {
        // Arrange
        _authorizationService.Setup(x => x.IsAuthorizedAsync(ProviderFeature.FlexiblePaymentsPilot)).ReturnsAsync(expectedResult);
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