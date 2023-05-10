namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Models
{
    /// <summary>
    /// model to hold client configurations of DfESignIn.
    /// </summary>
    public class DfESignInClientConfig
    {
        /// <summary>
        /// Gets or Sets ClientId.
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        ///  Gets or Sets ApiServiceSecret.
        /// </summary>
        public string ApiServiceSecret { get; set; }
        /// <summary>
        ///  Gets or Sets Secret.
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        ///  Gets or Sets ApiServiceId.
        /// </summary>
        public string ApiServiceId { get; set; }
    }
}
