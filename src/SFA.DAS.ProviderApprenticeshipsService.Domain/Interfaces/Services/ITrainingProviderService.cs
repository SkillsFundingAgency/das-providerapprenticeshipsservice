using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.TrainingProvider;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services
{
    /// <summary>
    /// Interface to define the contracts related to Training Provider services.
    /// </summary>
    public interface ITrainingProviderService
    {
        /// <summary>
        /// Contract to get the details of Provider from Outer API by given ukprn number.
        /// </summary>
        /// <param name="ukprn">ukprn number.</param>
        /// <returns></returns>
        Task<GetProviderResponseItem> GetProviderDetails(long ukprn);
    }
}
