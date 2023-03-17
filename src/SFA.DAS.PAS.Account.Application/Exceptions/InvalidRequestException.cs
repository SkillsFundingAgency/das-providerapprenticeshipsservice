using FluentValidation.Results;

namespace SFA.DAS.PAS.Account.Application.Exceptions
{
    public class InvalidRequestException : Exception
    {
        public Dictionary<string, string> ErrorMessages { get; private set; }

        public InvalidRequestException(Dictionary<string, string> errorMessages)
            : base(BuildErrorMessage(errorMessages))
        {
            this.ErrorMessages = errorMessages;
        }

        public InvalidRequestException(IEnumerable<ValidationFailure> failures)
            : this(failures.ToDictionary(failure => failure.PropertyName, failure => failure.ErrorMessage))
        {

        }

        private static string BuildErrorMessage(Dictionary<string, string> errorMessages)
        {
            if (errorMessages.Count == 0)
            {
                return "Request is invalid";
            }
            return "Request is invalid:\n"
                   + errorMessages.Select(kvp => $"{kvp.Key}: {kvp.Value}").Aggregate((x, y) => $"{x}\n{y}");
        }
    }
}
