using System;

using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text
{
    public struct ValidationMessage
    {

        public ValidationMessage(string text, string errorCode)
        {
            ErrorCode = errorCode;
            Text = text.RemoveHtmlTags();
        }

        public ValidationMessage(string text, string errorCode, bool withHtml)
        {
            if(string.IsNullOrEmpty(text))
                throw new ArgumentException($"Text missing for error code {errorCode}");
            if (string.IsNullOrEmpty(errorCode))
                throw new ArgumentException($"Error code missing for text {text}");
            ErrorCode = errorCode;
            Text = withHtml ? text : text.RemoveHtmlTags();
        }

        public string Text { get; }

        public string ErrorCode { get; }
    }

}