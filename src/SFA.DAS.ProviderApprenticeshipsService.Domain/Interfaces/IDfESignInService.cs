using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IDfESignInService
    {
        /// <summary>
        /// method to get the response from remote source.
        /// </summary>
        /// <typeparam name="T">Response model.</typeparam>
        /// <param name="userOrgId">User Org Id.</param>
        /// <param name="userId">User Id.</param>
        /// <returns>Response model.</returns>
        Task<T> Get<T>(string userOrgId, string userId);
    }
}
