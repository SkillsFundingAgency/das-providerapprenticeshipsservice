using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.TrainingProvider;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services
{
    /// <summary>
    /// Contract to interact with Training Provider(RoATP/APAR) Outer Api.
    /// </summary>
    public interface ITrainingProviderApiClient
    {
        /// <summary>
        /// Contract to get the details of the Provider by given ukprn or provider Id.
        /// </summary>
        /// <param name="providerId">ukprn.</param>
        /// <returns>GetProviderSummaryResult.</returns>
        Task<GetProviderSummaryResult> GetProviderDetails(long providerId);
    }
}
