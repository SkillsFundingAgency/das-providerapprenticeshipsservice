using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.CookieStorageService;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;

[Authorize]
[ServiceFilter(typeof(ProviderUkPrnCheckActionFilter))]
[SetNavigationSection(NavigationSection.Agreements)]
[Route("{providerId}/agreements")]
public class AgreementController : BaseController
{
    private readonly IProviderPRWebConfiguration _providerPRWebConfiguration;

    public AgreementController(ICookieStorageService<FlashMessageViewModel> flashMessage, IProviderPRWebConfiguration providerPRWebConfiguration) : base(flashMessage)
    {
        _providerPRWebConfiguration = providerPRWebConfiguration;
    }

    [HttpGet]
    [Route("", Name = RouteNames.GetAgreements)]
    public IActionResult Agreements(long providerId, string organisation = "")
    {
        return RedirectPermanent(_providerPRWebConfiguration.BaseUrl);
    }
}