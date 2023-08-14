using SFA.DAS.ProviderApprenticeshipsService.Application.Services.LinkGeneratorService;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

public static class ProviderUrlHelperExtensions
{
    public static string TraineeshipLink(this IUrlHelper helper, string path)
    {
        var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);

        return linkGenerator.TraineeshipLink(path);
    }

    public static string ProviderFundingLink(this IUrlHelper helper, string path)
    {
        var linkGenerator = GetLinkGenerator(helper.ActionContext.HttpContext);
        return linkGenerator.ProviderFundingLink(path);
    }

    private static ILinkGeneratorService GetLinkGenerator(HttpContext httpContext)
    {
        return httpContext.RequestServices.GetService(typeof(ILinkGeneratorService)) as ILinkGeneratorService;
    }
}