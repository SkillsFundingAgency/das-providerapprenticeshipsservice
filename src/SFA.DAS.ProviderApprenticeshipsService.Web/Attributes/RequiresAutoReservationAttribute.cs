using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes
{
    public class RequiresAutoReservationAttribute : Attribute
    {
        /// <summary>
        ///     Constructor that expects a method parameter named "hashedcommitmentid" to contain
        ///     the hash of a commitment.
        /// </summary>
        public RequiresAutoReservationAttribute() : this("hashedcommitmentid","providerId")
        {

        }

        /// <summary>
        ///     Constructor that expects a method parameter with the specified name to contain
        ///     the hash of a commitment.
        /// </summary>
        public RequiresAutoReservationAttribute(string hashedCommitmentIdField, string providerIdProperty)
        {
            if (string.IsNullOrWhiteSpace(hashedCommitmentIdField))
            {
                throw new ArgumentNullException(nameof(hashedCommitmentIdField), $"The attribute {nameof(RequiresAutoReservationAttribute)} must be supplied the name of the route parameter that contains the hashed commitment Id");
            }

            if (string.IsNullOrWhiteSpace(providerIdProperty))
            {
                throw new ArgumentNullException(nameof(providerIdProperty), $"The attribute {nameof(RequiresAutoReservationAttribute)} must be supplied the name of the route parameter that contains the providerIdProperty Id");
            }

            HashedCommitmentIdField = hashedCommitmentIdField;
        }

        /// <summary>
        ///     The name of the action parameter which contains the hashed commitment Id.
        /// </summary>
        public string HashedCommitmentIdField { get; }

        /// <summary>
        ///     The name of the action parameter which contains the provider Id.
        /// </summary>
        public string ProviderIdField { get; }
    }
}