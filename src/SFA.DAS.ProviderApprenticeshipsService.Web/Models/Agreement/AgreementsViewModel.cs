using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement
{
    public class AgreementsViewModel
    {
        public IEnumerable<CommitmentAgreement> CommitmentAgreements;
        public string SearchText { get; set; }
    }
}