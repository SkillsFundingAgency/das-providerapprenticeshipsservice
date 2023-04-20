using Microsoft.AspNetCore.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.CookieStorageService;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.RequireAuthenticatedUser))]
[Authorize(Policy = nameof(PolicyNames.RequireDasPermissionRole))]
public abstract class BaseController : Controller
{
    private const string FlashMessageCookieName = "sfa-das-providerapprenticeshipsservice-flashmessage";
    private readonly ICookieStorageService<FlashMessageViewModel> _flashMessage;

    protected BaseController(ICookieStorageService<FlashMessageViewModel> flashMessage)
    {
        _flashMessage = flashMessage;
    }

    protected string CurrentUserId;

    protected void SetInfoMessage(string messageText, FlashMessageSeverityLevel level)
    {
        var message = new FlashMessageViewModel
        {
            Message = messageText,
            Severity = level
        };
        _flashMessage.Delete(FlashMessageCookieName);

        _flashMessage.Create(message, FlashMessageCookieName);
    }

    public FlashMessageViewModel GetFlashMessageViewModelFromCookie()
    {
        var flashMessageViewModelFromCookie = _flashMessage.Get(FlashMessageCookieName);
        _flashMessage.Delete(FlashMessageCookieName);
        return flashMessageViewModelFromCookie;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (HttpContext.User.Identity.IsAuthenticated)
        {
            CurrentUserId = context.HttpContext.GetClaimValue(DasClaimTypes.Upn);
        }
    }
}