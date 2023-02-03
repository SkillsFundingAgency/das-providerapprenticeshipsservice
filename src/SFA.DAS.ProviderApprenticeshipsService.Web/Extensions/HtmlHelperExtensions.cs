using System;
using System.Linq;
using System.Linq.Expressions;
using MediatR;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.ProviderApprenticeshipsService.Application.Helpers;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetClientContent;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString AddClassIfPropertyInError<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string errorClass)
        {
            var expressionText = ExpressionHelper.GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var state = htmlHelper.ViewData.ModelState[fullHtmlFieldName];

            if (state?.Errors == null || state.Errors.Count == 0)
            {
                return HtmlString.Empty;
            }

            return new HtmlString(errorClass);
        }

        public static HtmlString AddClassIfPropertyInError<TModel>(
            this HtmlHelper<TModel> htmlHelper,
            string expressionText,
            string errorClass)
        {
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var state = htmlHelper.ViewData.ModelState[fullHtmlFieldName];

            if (state?.Errors == null || state.Errors.Count == 0)
            {
                return HtmlString.Empty;
            }

            return new HtmlString(errorClass);
        }
        
        public static HtmlString DasValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var propertyName = ExpressionHelper.GetExpressionText(expression);

            if (htmlHelper.ViewData.ModelState.IsValidField(propertyName))
            {
                return new HtmlString(string.Empty);
            }

            var error = htmlHelper.ViewData.ModelState[propertyName].Errors.First();
            var errorMesage = ValidationMessage.ExtractFieldMessage(error.ErrorMessage);
            
            var builder = new TagBuilder("span");

            builder.AddCssClass("error-message");
            builder.AddCssClass("field-validation-error");
            builder.Attributes.Add("id", $"error-message-{propertyName}");
            builder.SetInnerText(errorMesage);

            return new HtmlString(builder.ToString());
        }

        public static HtmlString GetClientContentByType(this IHtmlHelper html, string type, bool useLegacyStyles = false)
        {
            var mediator = DependencyResolver.Current.GetService<IMediator>();

            IMediator Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            var userResponse = AsyncHelper.RunSync(() => mediator.Send(new GetClientContentRequest
            {
                UseLegacyStyles = useLegacyStyles,
                ContentType = type
            }));            

            var content = userResponse;
            return HtmlString.Create(content.Content);
        }

        public static HtmlString SetZenDeskLabels(this IHtmlHelper html, params string[] labels)
        {
            var keywords = string.Join(",", labels
                .Where(label => !string.IsNullOrEmpty(label))
                .Select(label => $"'{EscapeApostrophes(label)}'"));

            // when there are no keywords default to empty string to prevent zen desk matching articles from the url
            var apiCallString = "<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', { labels: ["
                                + (!string.IsNullOrEmpty(keywords) ? keywords : "''")
                                + "] });</script>";

            return HtmlString.Create(apiCallString);
        }

        private static string EscapeApostrophes(string input)
        {
            return input.Replace("'", @"\'");
        }

        public static string GetZenDeskSnippetKey(this IHtmlHelper html)
        {
            var configuration = DependencyResolver.Current.GetService<ProviderApprenticeshipsServiceConfiguration>();
            return configuration.ZenDeskSettings.SnippetKey;
        }

        public static string GetZenDeskSnippetSectionId(this IHtmlHelper html)
        {
            var configuration = DependencyResolver.Current.GetService<ProviderApprenticeshipsServiceConfiguration>();
            return configuration.ZenDeskSettings.SectionId;
        }
        public static string GetZenDeskCobrowsingSnippetKey(this IHtmlHelper html)
        {
            var configuration = DependencyResolver.Current.GetService<ProviderApprenticeshipsServiceConfiguration>();
            return configuration.ZenDeskSettings.CobrowsingSnippetKey;
        }
    }
}