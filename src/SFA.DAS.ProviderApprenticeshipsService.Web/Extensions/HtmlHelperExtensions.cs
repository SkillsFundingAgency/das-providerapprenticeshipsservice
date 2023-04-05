using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Html;
using SFA.DAS.ProviderApprenticeshipsService.Application.Helpers;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetClientContent;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public interface IHtmlHelpers
    {
        HtmlString GetClientContentByType(string type, bool useLegacyStyles = false);
        HtmlString SetZenDeskLabels(params string[] labels);
        string GetZenDeskSnippetKey();
        string GetZenDeskSnippetSectionId();
        string GetZenDeskCobrowsingSnippetKey();
    }

    public class HtmlHelpers : IHtmlHelpers
    {
        private readonly ProviderApprenticeshipsServiceConfiguration _configuration;
        private readonly IMediator _mediator;

        public HtmlHelpers(
        ProviderApprenticeshipsServiceConfiguration configuration,
        IMediator mediator)
        {
            _configuration = configuration;
            _mediator = mediator;
        }

        public HtmlString GetClientContentByType(string type, bool useLegacyStyles = false)
        {
            var userResponse = AsyncHelper.RunSync(() => _mediator.Send(new GetClientContentRequest
            {
                UseLegacyStyles = useLegacyStyles,
                ContentType = type
            }));

            return new HtmlString(userResponse.Content);
        }

        public HtmlString SetZenDeskLabels(params string[] labels)
        {
            var keywords = string.Join(",", labels
                .Where(label => !string.IsNullOrEmpty(label))
                .Select(label => $"'{EscapeApostrophes(label)}'"));

            // when there are no keywords default to empty string to prevent zen desk matching articles from the url
            var apiCallString = "<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', { labels: ["
                                + (!string.IsNullOrEmpty(keywords) ? keywords : "''")
                                + "] });</script>";

            return new HtmlString(apiCallString);
        }

        private static string EscapeApostrophes(string input)
        {
            return input.Replace("'", @"\'");
        }

        public string GetZenDeskSnippetKey()
        {
            return _configuration.ZenDeskSettings.SnippetKey;
        }

        public string GetZenDeskSnippetSectionId()
        {
            return _configuration.ZenDeskSettings.SectionId;
        }
        public string GetZenDeskCobrowsingSnippetKey()
        {
            return _configuration.ZenDeskSettings.CobrowsingSnippetKey;
        }
    }
}