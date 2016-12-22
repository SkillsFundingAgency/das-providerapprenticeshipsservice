namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text
{
    public struct ValidationMessage
    {
        public ValidationMessage(string text, string errorCode)
        {
            ErrorCode = errorCode;
            Text = text;
        }

        public string Text { get; }

        public string ErrorCode { get; }
    }
}