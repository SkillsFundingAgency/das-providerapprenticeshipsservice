namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.Account
{
    public class ChangeOfDetailsViewModel
    {
        private readonly string _integrationUrlPart;

        public ChangeOfDetailsViewModel(string environment)
        {
            switch (!string.IsNullOrEmpty(environment))
            {
                case true when environment.Equals("prod", StringComparison.CurrentCultureIgnoreCase):
                case true when environment.Equals("prd", StringComparison.CurrentCultureIgnoreCase):
                    _integrationUrlPart = string.Empty;
                    break;
                case true when environment.Equals("preprod", StringComparison.CurrentCultureIgnoreCase):
                    _integrationUrlPart = "pp-";
                    break;
                default:
                    _integrationUrlPart = "test-";
                    break;
            }
        }

        /// <summary>
        /// Gets DfESignIn profile service link.
        /// </summary>
        public string ProfilePageLink => $"https://{_integrationUrlPart}profile.signin.education.gov.uk/";
    }
}
