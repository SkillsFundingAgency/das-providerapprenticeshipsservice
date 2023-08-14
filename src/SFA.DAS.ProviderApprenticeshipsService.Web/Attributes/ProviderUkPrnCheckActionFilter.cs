using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;

public class ProviderUkPrnCheckActionFilter : IActionFilter
{
    private const string ProviderIdKey = "providerId";

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ActionArguments.ContainsKey(ProviderIdKey))
        {
            return;
        }

        var providerIdFromAction = context.ActionArguments[ProviderIdKey];

        if ($"{providerIdFromAction}" == string.Empty)
        {
            throw new HttpRequestException("Missing provider id", null, HttpStatusCode.BadRequest);
        }

        var claimUkprn = context.HttpContext.GetClaimValue(DasClaimTypes.Ukprn);

        if ($"{providerIdFromAction}" != claimUkprn)
        {
            throw new HttpRequestException($"Mismatched UKPRNs ({providerIdFromAction} requested on URL, user's claim contains {claimUkprn}", null, HttpStatusCode.Forbidden);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // empty but required for filter interface and sonarcloud inspection.
    }
}