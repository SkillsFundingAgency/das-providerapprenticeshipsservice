using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IProviderAgreementStatusConfiguration : IBaseConfiguration
    {
        bool CheckForContractAgreements { get; set; }
    }
}
