namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

public struct ValidationMessage
{
    public ValidationMessage(string text, string errorCode)
    {
        ErrorCode = errorCode;
        Text = text;
    }

    // might be better to have an IValidationMessage, and have a separate ComposableValidationMessage, so the original ValidationMessage can't be used incorrectly
    public ValidationMessage(ValidationMessage composableValidationMessage, params object[] args)
    {
        ErrorCode = composableValidationMessage.ErrorCode;
        Text = string.Format(composableValidationMessage.Text, args);
    }

    public string Text { get; }

    public string ErrorCode { get; }

    public static string ExtractBannerMessage(string errorMessage)
    {
        var seperatorIndex = errorMessage.IndexOf("||");
        return seperatorIndex == -1 ? errorMessage : errorMessage.Substring(0, seperatorIndex);
    }

    public static string ExtractFieldMessage(string errorMessage)
    {
        var seperatorIndex = errorMessage.IndexOf("||");
        return seperatorIndex == -1 ? errorMessage : errorMessage.Substring(seperatorIndex + 2);
    }
}