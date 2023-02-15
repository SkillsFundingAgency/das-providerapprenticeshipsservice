using System;
using System.Linq;
using System.Linq.Expressions;
using MediatR;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Application.Helpers;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetClientContent;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public interface IHtmlHelpers
    {
        HtmlString AddClassIfPropertyInError<TModel, TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            string errorClass);
        HtmlString AddClassIfPropertyInError<TModel>(
            string expressionText,
            string errorClass);
        HtmlString DasValidationMessageFor<TModel, TProperty>( 
            Expression<Func<TModel, TProperty>> expression);
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HtmlHelpers> _logger;
        private readonly IHtmlHelper _htmlHelper;
        private ModelExpressionProvider _expressionProvider { get; }

        public HtmlHelpers(
        ProviderApprenticeshipsServiceConfiguration configuration,
        IMediator mediator,
        IHttpContextAccessor httpContextAccessor,
        ILogger<HtmlHelpers> logger,
        IHtmlHelper htmlHelper)
        {
            _configuration = configuration;
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _htmlHelper = htmlHelper;
            _expressionProvider = _htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider; ;
        }

        public HtmlString AddClassIfPropertyInError<TModel, TProperty>(
            Expression<Func<TModel, TProperty>> expression,
            string errorClass)
        {
            var expressionText = _expressionProvider.GetExpressionText(expression);
            var fullHtmlFieldName = _htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var state = _htmlHelper.ViewData.ModelState[fullHtmlFieldName];

            if (state?.Errors == null || state.Errors.Count == 0)
            {
                return HtmlString.Empty;
            }

            return new HtmlString(errorClass);
        }

        public HtmlString AddClassIfPropertyInError<TModel>(
            string expressionText,
            string errorClass)
        {
            var fullHtmlFieldName = _htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var state = _htmlHelper.ViewData.ModelState[fullHtmlFieldName];

            if (state?.Errors == null || state.Errors.Count == 0)
            {
                return HtmlString.Empty;
            }

            return new HtmlString(errorClass);
        }
        
        public HtmlString DasValidationMessageFor<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var propertyName = _expressionProvider.GetExpressionText(expression);

            if (_htmlHelper.ViewData.ModelState.GetValidationState(propertyName).Equals(ModelValidationState.Valid))
            {
                return new HtmlString(string.Empty);
            }

            var error = _htmlHelper.ViewData.ModelState[propertyName].Errors.First();
            var errorMesage = ValidationMessage.ExtractFieldMessage(error.ErrorMessage);
            
            var builder = new TagBuilder("span");

            builder.AddCssClass("error-message");
            builder.AddCssClass("field-validation-error");
            builder.Attributes.Add("id", $"error-message-{propertyName}");
            builder.InnerHtml.SetContent(errorMesage);

            return new HtmlString(builder.ToString());
        }

        public HtmlString GetClientContentByType(string type, bool useLegacyStyles = false)
        {
            var userResponse = AsyncHelper.RunSync(() => _mediator.Send(new GetClientContentRequest
            {
                UseLegacyStyles = useLegacyStyles,
                ContentType = type
            }));            

            var content = userResponse;
            return new HtmlString(content.Content);
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

        private string EscapeApostrophes(string input)
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