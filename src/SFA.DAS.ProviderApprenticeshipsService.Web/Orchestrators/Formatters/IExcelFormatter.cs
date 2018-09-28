using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Formatters
{
    public interface IExcelFormatter
    {
        byte[] Format(IEnumerable<CommitmentAgreement> commitmentAgreements);
    }
}