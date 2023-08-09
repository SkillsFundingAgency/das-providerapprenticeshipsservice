namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.Home
{
    public class HomeViewModel
    {
        private readonly string _integrationUrlPart;

        public HomeViewModel(string environment)
        {
            _integrationUrlPart = environment.ToLowerInvariant() switch
            {
                "local" => "test-",
                "test" => "test-",
                "pp" => "pp-",
                "preprod" => "pp-",
                "prd" => string.Empty,
                _ => string.Empty
            };
        }

        public string ResetPasswordLink => $"https://{_integrationUrlPart}interactions.signin.education.gov.uk/{Guid.NewGuid()}/resetpassword/request?clientid=services&redirect_uri=https://{_integrationUrlPart}services.signin.education.gov.uk:443/auth/cb";
    }
}
