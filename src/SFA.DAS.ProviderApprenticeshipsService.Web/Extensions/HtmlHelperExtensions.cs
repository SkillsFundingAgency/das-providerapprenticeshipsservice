using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString AddClassIfPropertyInError<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string errorClass)
        {
            var expressionText = ExpressionHelper.GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var state = htmlHelper.ViewData.ModelState[fullHtmlFieldName];

            if (state?.Errors == null || state.Errors.Count == 0)
            {
                return MvcHtmlString.Empty;
            }

            return new MvcHtmlString(errorClass);
        }

        public static MvcHtmlString AddClassIfPropertyInError<TModel>(
            this HtmlHelper<TModel> htmlHelper,
            string expressionText,
            string errorClass)
        {
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var state = htmlHelper.ViewData.ModelState[fullHtmlFieldName];

            if (state?.Errors == null || state.Errors.Count == 0)
            {
                return MvcHtmlString.Empty;
            }

            return new MvcHtmlString(errorClass);
        }
        
        public static MvcHtmlString DasValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var propertyName = ExpressionHelper.GetExpressionText(expression);

            if (htmlHelper.ViewData.ModelState.IsValidField(propertyName))
            {
                return new MvcHtmlString(string.Empty);
            }

            var error = htmlHelper.ViewData.ModelState[propertyName].Errors.First();
            var errorMesage = ValidationMessage.ExtractFieldMessage(error.ErrorMessage);
            
            var builder = new TagBuilder("span");

            builder.AddCssClass("error-message");
            builder.AddCssClass("field-validation-error");
            builder.Attributes.Add("id", $"error-message-{propertyName}");
            builder.SetInnerText(errorMesage);

            return new MvcHtmlString(builder.ToString());
        }
    }
}