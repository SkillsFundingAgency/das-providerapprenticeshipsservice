namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Models
{
    /// <summary>
    /// model to hold configurations of DfESignIn
    /// </summary>
    public class DfESignInConfig
    {
        /// <summary>
        /// Gets or Sets BaseUrl
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or Sets Client Identifier.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or Sets Secret.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or Sets Api Service Url.
        /// </summary>
        public string ApiServiceUrl { get; set; }

        /// <summary>
        /// Gets or Sets Api Service Id.
        /// </summary>
        public string ApiServiceId { get; set; }

        /// <summary>
        /// Gets or Sets Api Service Secret.
        /// </summary>
        public string ApiServiceSecret { get; set; }
    }
}
