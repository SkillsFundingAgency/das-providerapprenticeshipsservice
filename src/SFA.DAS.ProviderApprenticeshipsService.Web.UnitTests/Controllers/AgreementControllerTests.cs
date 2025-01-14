using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.CookieStorageService;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Controllers;

public sealed class AgreementControllerTests
{
    private Mock<ICookieStorageService<FlashMessageViewModel>> _mockFlashMessageService;
    private Mock<IProviderPRWebConfiguration> _mockProviderPRWebConfiguration;
    private AgreementController _sut;

    private const string ProviderRelationshipsBaseUrl = "provider-relationships-base-url";

    [SetUp]
    public void Setup()
    {
        _mockFlashMessageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
        _mockProviderPRWebConfiguration = new Mock<IProviderPRWebConfiguration>();
        _mockProviderPRWebConfiguration.Setup(c => c.BaseUrl).Returns(ProviderRelationshipsBaseUrl);

        _sut = new AgreementController(_mockFlashMessageService.Object, _mockProviderPRWebConfiguration.Object);
    }

    public void Agreements_ShouldPermanentlyRedirect_ToProviderRelationshipsWebBaseUrl()
    {
        var result = _sut.Agreements(12345, "TestOrganisation") as RedirectResult;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<RedirectResult>());
            Assert.That(ProviderRelationshipsBaseUrl, Is.EqualTo(result.Url));
        });
    }
}
